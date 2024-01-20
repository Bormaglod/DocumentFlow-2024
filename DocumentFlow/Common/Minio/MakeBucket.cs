//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Microsoft.Extensions.Logging;

using Minio;
using Minio.DataModel.Args;

namespace DocumentFlow.Common.Minio;

public static class MakeBucket
{
    public static async Task Run(IMinioClient minio, string bucketName)
    {
        if (minio is null)
        {
            throw new ArgumentNullException(nameof(minio));
        }

        await minio.MakeBucketAsync(
            new MakeBucketArgs()
                .WithBucket(bucketName)
        ).ConfigureAwait(false);

        var logger = ServiceLocator.Context.GetService<ILogger<MinioClient>>();
        logger?.LogInformation($"Created bucket {bucketName}");
    }
}
