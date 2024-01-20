//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Microsoft.Extensions.Logging;

using Minio;
using Minio.DataModel.Args;

using System.IO;

namespace DocumentFlow.Common.Minio;

public class GetObjectStream
{
    public static async Task Run(IMinioClient minio,
        string bucketName,
        string objectName,
        Action<Stream> callbackStream)
    {
        if (minio is null)
        {
            throw new ArgumentNullException(nameof(minio));
        }

        var args = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithCallbackStream(callbackStream);
        _ = await minio.GetObjectAsync(args).ConfigureAwait(false);

        var logger = ServiceLocator.Context.GetService<ILogger<MinioClient>>();
        logger?.LogInformation($"Downloaded the object {objectName} in bucket {bucketName}");
    }
}