//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common;
using DocumentFlow.Models.Entities;

using Minio;

using Syncfusion.Data;
using Syncfusion.UI.Xaml.Utility;

using System.Windows.Input;

namespace DocumentFlow.Commands;

public static class EntityGridCommands
{
    #region OpenImage

    static ICommand? openImage;

    public static ICommand OpenImage
    {
        get
        {
            openImage ??= new BaseCommand(OnOpenImage);
            return openImage;
        }
    }

    private static void OnOpenImage(object parameter)
    {
        if (parameter is List<object> list) 
        {
            string? bucket = null;
            string? fileName = null;
            string? s3object = null;
            foreach (var item in list)
            {
                switch (item)
                {
                    case RecordEntry entry:
                        if (entry.Data is DocumentInfo info)
                        {
                            bucket = DocumentRefs.GetBucketForEntity(info);
                        }

                        break;
                    case DocumentRefs refs:
                        fileName = refs.FileName;
                        s3object = refs.S3object;
                        break;
                }
            }

            if (!string.IsNullOrEmpty(bucket) && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(s3object))
            {
                FileHelper.OpenFile(ServiceLocator.Context.GetService<IMinioClient>(), fileName, bucket, s3object);
            }
        }
    }

    #endregion
}
