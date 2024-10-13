namespace CrawlerStorage.Services;

using System.Text;
using System.Threading.Tasks;

using CrawlerStorage.Data.Models.Http;
using CrawlerStorage.Services.Intrefaces;

using HtmlAgilityPack;

public class HttpService : IHttpService
{
    private readonly HttpClient httpClient;

    public HttpService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<HttpModel> GetAsync(string url, int crawlerId)
    {
        var response = await this.httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var httpModel = await this.CreateHttpModelAsync(response, url, crawlerId);
            return httpModel;
        }

        return null;
    }

    private async Task<HttpModel> CreateHttpModelAsync(HttpResponseMessage response, string url, int crawlerId)
    {
        var httpModel = new HttpModel
        {
            Url = url,
            MimeType = response.Content.Headers.ContentType?.MediaType,
            Content = await this.ReadContentAsync(response, url, crawlerId),
            Encoding = Encoding.UTF8,
            Name = this.CreateNameFromUrl(url)
        };

        var charSet = response.Content.Headers.ContentType?.CharSet;
        if (charSet != null && charSet.ToLower() != "utf-8")
        {
            httpModel.Encoding = Encoding.GetEncoding(charSet);
        }

        return httpModel;
    }

    private async Task<byte[]> ReadContentAsync(HttpResponseMessage response, string url, int crawlerId)
    {
        var content = await response.Content.ReadAsByteArrayAsync();
        switch (crawlerId)
        {
            case 1:
                var responseAsString = await response.Content.ReadAsStringAsync();
                while (responseAsString == "Rate Limit Exceeded")
                {
                    await Task.Delay(3000);
                    response = await this.httpClient.GetAsync(url);
                    responseAsString = await response.Content.ReadAsStringAsync();
                }

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(responseAsString);

                var html = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='container']");
                if (html != null)
                {
                    content = Encoding.UTF8.GetBytes(html.OuterHtml);
                }

                break;
        }

        return content;
    }

    private string CreateNameFromUrl(string url)
    {
        var uri = new Uri(url);
        return $"{uri.Host}_{string.Join("_", uri.Segments.Where(x => x != "/").Select(x => x.Replace("/", string.Empty)))}".ToLower();
    }
}