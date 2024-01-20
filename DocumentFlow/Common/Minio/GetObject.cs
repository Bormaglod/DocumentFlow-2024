//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Microsoft.Extensions.Logging;

using Minio;
using Minio.DataModel.Args;

namespace DocumentFlow.Common.Minio;

public class GetObject
{
    public static async Task Run(IMinioClient minio,
        string bucketName,
        string objectName,
        string fileName)
    {
        if (minio is null)
        {
            throw new ArgumentNullException(nameof(minio));
        }

        var args = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithFile(fileName);
        _ = await minio.GetObjectAsync(args).ConfigureAwait(false);

        var logger = ServiceLocator.Context.GetService<ILogger<MinioClient>>();
        logger?.LogInformation($"Downloaded the object {objectName} in bucket {bucketName}");
    }
}
