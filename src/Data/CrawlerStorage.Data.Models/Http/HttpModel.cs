namespace CrawlerStorage.Data.Models.Http;

using System.Text;

public class HttpModel
{
    public string Name { get; set; }

    public string Url { get; set; }

    public Uri Uri => new Uri(this.Url, UriKind.RelativeOrAbsolute);

    //public byte[] Bytes => this.Encoding.GetBytes(this.Content);

    public Encoding Encoding { get; set; }

    public byte[] Content { get; set; }

    public string MimeType { get; set; }

    //    public HtmlDocument HtmlDocument { get; set; }

    //    public HttpStatusCode StatusCode { get; set; }
}