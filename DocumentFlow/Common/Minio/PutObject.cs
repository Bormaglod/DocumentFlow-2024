//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.HighPerformance;

using Microsoft.Extensions.Logging;

using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Encryption;

using System.IO;

namespace DocumentFlow.Common.Minio;

public static class PutObject
{
    public static async Task Run(IMinioClient minio,
        string bucketName,
        string objectName,
        string fileName,
        IProgress<ProgressReport>? progress = null,
        IServerSideEncryption? sse = null)
    {
        if (minio is null)
        {
            throw new ArgumentNullException(nameof(minio));
        }

        ReadOnlyMemory<byte> bs = await File.ReadAllBytesAsync(fileName).ConfigureAwait(false);
        using var filestream = bs.AsStream();

        var args = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(filestream)
            .WithObjectSize(filestream.Length)
            .WithContentType("application/octet-stream")
            .WithProgress(progress)
            .WithServerSideEncryption(sse);
        _ = await minio.PutObjectAsync(args).ConfigureAwait(false);

        var logger = ServiceLocator.Context.GetService<ILogger<MinioClient>>();
        logger?.LogInformation($"Uploaded object {objectName} to bucket {bucketName}");
    }
}