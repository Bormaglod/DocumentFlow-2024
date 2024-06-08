//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Input;

using DocumentFlow.Common;
using DocumentFlow.Common.Minio;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Minio;

using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;

using System.Diagnostics.CodeAnalysis;

namespace DocumentFlow.Commands;

public static partial class FileCommands
{
    #region OpenFileCommand

    private static RelayCommand<object>? openFileCommand;

    public static IRelayCommand<object> OpenFileCommand => openFileCommand ??= new RelayCommand<object>(OpenFile);

    private static void OpenFile(object? parameter)
    {
        if (GetFileParameters(parameter, out var bucket, out var fileName, out var s3object)) 
        {
            FileHelper.OpenFile(ServiceLocator.Context.GetService<IMinioClient>(), fileName, bucket, s3object);
        }
    }

    #endregion

    #region SaveFileCommand

    private static RelayCommand<object>? saveFileCommand;

    public static IRelayCommand<object> SaveFileCommand => saveFileCommand ??= new RelayCommand<object>(SaveFile);

    private static async void SaveFile(object? parameter)
    {
        if (GetFileParameters(parameter, out var bucket, out var fileName, out var s3object))
        {
            Microsoft.Win32.SaveFileDialog dlg = new()
            {
                FileName = fileName,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dlg.ShowDialog() == true)
            {
                var minio = ServiceLocator.Context.GetService<IMinioClient>();
                await GetObject.Run(minio, bucket, s3object, dlg.FileName)
                    .ContinueWith(task =>
                    {
                        ToastOperations.DownloadFileCompleted(dlg.FileName);
                    });
            }
        }
    }

    #endregion

    #region Common methods

    private static bool GetFileParameters(object? parameter, [MaybeNullWhen(false)] out string bucket, [MaybeNullWhen(false)] out string fileName, [MaybeNullWhen(false)] out string s3object)
    {
        bucket = null;
        fileName = null;
        s3object = null;

        switch (parameter)
        {
            case OpenDocumentContext context:
                bucket = DocumentRefs.GetBucketForEntity(context.Owner);
                fileName = context.Document.FileName;
                s3object = context.Document.S3object;

                break;
            case GridRecordContextMenuInfo menuInfo:
                if (menuInfo.DataGrid.DataContext is IEntityEditorViewModel model && model.DocumentInfo != null)
                {
                    bucket = DocumentRefs.GetBucketForEntity(model.DocumentInfo);
                }

                if (menuInfo.Record is DocumentRefs dr)
                {
                    fileName = dr.FileName;
                    s3object = dr.S3object;
                }

                break;
            case List<object> list:
                foreach (var item in list)
                {
                    switch (item)
                    {
                        case DocumentInfo document:
                            bucket = DocumentRefs.GetBucketForEntity(document);
                            break;
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

                break;
        }

        return !(string.IsNullOrEmpty(bucket) || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(s3object));
    }

    #endregion
}
