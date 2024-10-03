namespace CrawlerStorage.Common.Helpers;

using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Zip;

using ICSharpCode.SharpZipLib.Zip;

public static class ZipHelper
{
    public static List<ZipModel> UnzipGroup(byte[] data)
    {
        var zipModels = new List<ZipModel>();

        using var memoryStream = new MemoryStream(data);
        using var zipFile = new ZipFile(memoryStream);

        foreach (ZipEntry zipEntry in zipFile)
        {
            if (!zipEntry.IsFile)
            {
                continue;
            }

            var fileName = zipEntry.Name;
            var buffer = new byte[4096];
            var zipStream = zipFile.GetInputStream(zipEntry);

            using var ms = new MemoryStream();

            int read;
            while ((read = zipStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            var zipModel = new ZipModel
            {
                Name = fileName,
                Content = ms.ToArray()
            };

            zipModels.Add(zipModel);
        }

        return zipModels;
    }

    public static byte[] ZipGroup(Group group)
    {
        using var memoryStream = new MemoryStream();
        using var zipOutputStream = new ZipOutputStream(memoryStream);

        zipOutputStream.SetLevel(9);
        var buffer = new byte[4096];

        foreach (var document in group.Documents)
        {
            var entry = new ZipEntry(Path.GetFileName(document.Name));
            zipOutputStream.PutNextEntry(entry);

            using var ms = new MemoryStream(document.Content, 0, document.Content.Length);

            int bytes;
            do
            {
                bytes = ms.Read(buffer, 0, buffer.Length);
                zipOutputStream.Write(buffer, 0, bytes);
            }
            while (bytes > 0);
        }

        zipOutputStream.Finish();

        return memoryStream.ToArray();
    }
}