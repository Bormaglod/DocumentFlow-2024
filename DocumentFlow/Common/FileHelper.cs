//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

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

    private static string GetTempPath(string category) => Path.Combine(Path.GetTempPath(), "DocumentFlow", category);

    private static void DeleteTempFiles(string category)
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
}
