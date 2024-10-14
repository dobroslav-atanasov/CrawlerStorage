namespace CrawlerStorage.Services;

using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CrawlerStorage.Data.Models.Settings;
using CrawlerStorage.Services.Intrefaces;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IOptions<RabbitMQModel> options;
    private readonly IEventProcessor eventProcessor;
    private IConnection connection;
    private IModel channel;
    private string queueName;

    public MessageBusSubscriber(IOptions<RabbitMQModel> options, IEventProcessor eventProcessor)
    {
        this.options = options;
        this.eventProcessor = eventProcessor;

        this.InitializeRabbitMQ();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(this.channel);

        consumer.Received += (sender, args) =>
        {
            Console.WriteLine($"--> Event Received!");

            var body = args.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());

            this.eventProcessor.ProcessEvent(message);
            ;
            // this.eventProcessor .......
        };

        this.channel.BasicConsume(this.queueName, true, consumer);

        return Task.CompletedTask;
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory
        {
            HostName = this.options.Value.Host,
            Port = this.options.Value.Port,
            UserName = this.options.Value.User,
            Password = this.options.Value.Password
        };

        this.connection = factory.CreateConnection();
        this.channel = this.connection.CreateModel();

        this.channel.ExchangeDeclare("test", ExchangeType.Fanout);
        this.queueName = this.channel.QueueDeclare().QueueName;

        this.channel.QueueBind(this.queueName, "test", "");

        Console.WriteLine($"--> Listening on the Message Bus...");

        this.connection.ConnectionShutdown += this.RabbitMQConnectionShutdown;
    }

    private void RabbitMQConnectionShutdown(object sender, ShutdownEventArgs args)
    {
        Console.WriteLine($"--> Connection shutdown.");
    }

    public override void Dispose()
    {
        if (this.channel.IsOpen)
        {
            this.channel.Close();
            this.connection.Close();
        }
    }
}