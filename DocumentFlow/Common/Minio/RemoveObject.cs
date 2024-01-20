//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Microsoft.Extensions.Logging;

using Minio;
using Minio.DataModel.Args;

namespace DocumentFlow.Common.Minio;

public static class RemoveObject
{
    public static async Task Run(IMinioClient minio,
        string bucketName,
        string objectName,
        string? versionId = null)
    {
        if (minio is null)
        {
            throw new ArgumentNullException(nameof(minio));
        }

        var args = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);
        
        if (!string.IsNullOrEmpty(versionId))
        {
            args = args.WithVersionId(versionId);
        }

        await minio.RemoveObjectAsync(args).ConfigureAwait(false);

        var logger = ServiceLocator.Context.GetService<ILogger<MinioClient>>();
        logger?.LogInformation($"Removed object {objectName} from bucket {bucketName} successfully");
    }
}
