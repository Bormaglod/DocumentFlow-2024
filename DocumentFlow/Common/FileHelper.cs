//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Minio;

using Minio;

using System.IO;

namespace DocumentFlow.Common;

public static class FileHelper
{
    public static void OpenFile(IMinioClient client, string fileName, string bucket, string s3object)
    {
        string path = PrepareTempPath("DocumentRefs");
        string file = Path.Combine(path, fileName);

        GetObject.Run(client, bucket, s3object, file).Wait();
        WorkOperations.OpenFile(file);
    }

    public static string PrepareTempPath(string category)
    {
        DeleteTempFiles(category);
        string path = GetTempPath("DocumentRefs");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    public static void DeleteTempFiles(string category)
    {
        string path = GetTempPath(category);
        if (Directory.Exists(path))
        {
            var di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }
    }

    public static string GetTempFileName(string category, PdfNamingStrategy namingStrategy, FileExtension extension)
    {
        DeleteTempFiles(category);

        string path = Path.Combine(Path.GetTempPath(), "DocumentFlow", category);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return Path.Combine(path, GetTempFileName(namingStrategy, extension));
    }

    private static string GetTempFileName(PdfNamingStrategy namingStrategy, FileExtension extension)
    {
        return $"{GetTempFileName(namingStrategy)}.{extension.ToString().ToLower()}";
    }

    private static string GetTempFileName(PdfNamingStrategy namingStrategy)
    {
        return namingStrategy switch
        {
            PdfNamingStrategy.Guid => Guid.NewGuid().ToString(),
            PdfNamingStrategy.DateTime => $"SCN_{DateTime.Today:yyyyMMdd}_{(int)(DateTime.Now - DateTime.Today).TotalSeconds}",
            _ => throw new NotImplementedException()
        };
    }

    private static string GetTempPath(string category) => Path.Combine(Path.GetTempPath(), "DocumentFlow", category);
}
