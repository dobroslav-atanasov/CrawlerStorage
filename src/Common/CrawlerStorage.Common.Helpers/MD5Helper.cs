namespace CrawlerStorage.Common.Helpers;

using System.Security.Cryptography;
using System.Text;

public static class MD5Helper
{
    public static string Hash(byte[] data)
    {
        using var md5 = MD5.Create();

        var hashBytes = md5.ComputeHash(data);

        var sb = new StringBuilder();
        for (var i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("X2"));
        }

        return sb.ToString();
    }
}