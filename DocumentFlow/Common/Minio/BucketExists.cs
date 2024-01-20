//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Minio;
using Minio.DataModel.Args;

namespace DocumentFlow.Common.Minio;

public static class BucketExists
{
    public static async Task<bool> Run(IMinioClient minio, string bucketName)
    {
        if (minio is null)
        {
            throw new ArgumentNullException(nameof(minio));
        }

        var args = new BucketExistsArgs()
            .WithBucket(bucketName);
        return await minio.BucketExistsAsync(args).ConfigureAwait(false);
    }
}
