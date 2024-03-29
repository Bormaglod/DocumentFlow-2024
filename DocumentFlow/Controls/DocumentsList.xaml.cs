﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Common.Minio;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;
using DocumentFlow.Models.Settings;
using DocumentFlow.Tools;

using Microsoft.Extensions.Options;

using Minio;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DocumentFlow.Controls;

/// <summary>
/// Логика взаимодействия для DocumentsListView.xaml
/// </summary>
public partial class DocumentsList : UserControl
{
    public DocumentsList()
    {
        InitializeComponent();
    }

    public DocumentInfo DocumentInfo
    {
        get { return (DocumentInfo)GetValue(DocumentInfoProperty); }
        set { SetValue(DocumentInfoProperty, value); }
    }

    public static readonly DependencyProperty DocumentInfoProperty = DependencyProperty.Register(
        "DocumentInfo",
        typeof(DocumentInfo),
        typeof(DocumentsList),
        new FrameworkPropertyMetadata(OnDocumentInfoChanged));

    private static void OnDocumentInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        ((DocumentsList)d).gridContent.ItemsSource = new ObservableCollection<DocumentRefs>(conn.Query<DocumentRefs>("select * from document_refs where owner_id = :Id", e.NewValue));
    }

    private void AddDocumentRef(DocumentRefs document, string fileName, bool createThumbnail, string bucket)
    {
        document.S3object = $"{document.OwnerId}_{document.FileName}";

        var minio = ServiceLocator.Context.GetService<IMinioClient>();
        BucketExists.Run(minio, bucket)
            .ContinueWith(async task =>
            {
                if (task.Result)
                {
                    return;
                }

                await MakeBucket.Run(minio, bucket);
            })
            .ContinueWith(async task =>
            {
                await PutObject.Run(minio, bucket, document.S3object, fileName);
            })
            .Wait();

        if (createThumbnail)
        {
            var settings = ServiceLocator.Context.GetService<IOptions<LocalSettings>>().Value.PreviewRows.ThumbnailRow;

            document.CreateThumbnailImage(fileName, settings.ImageSize);
        }

        try
        {
            using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                conn.Insert(document, transaction);
                transaction.Commit();

                if (gridContent.ItemsSource is IList<DocumentRefs> list)
                {
                    list.Add(document);
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void EditDocumentRefs()
    {
        if (gridContent.SelectedItem is DocumentRefs refs)
        {
            var dialog = new DocumentRefWindow();
            if (dialog.Edit(refs))
            {
                try
                {
                    using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
                    using var transaction = conn.BeginTransaction();

                    try
                    {
                        conn.Execute("update document_refs set note = :Note where id = :Id", refs, transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void AddDocument(object sender, RoutedEventArgs e)
    {
        if (DocumentInfo == null)
        {
            MessageBox.Show("Для добавления сопутствующих документов необходимо сначала сохранить текущий", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dialog = new DocumentRefWindow();
        if (dialog.Create(DocumentInfo.Id, out var refs))
        {
            AddDocumentRef(refs, dialog.FileNameWithPath, dialog.CreateThumbnailImage, DocumentRefs.GetBucketForEntity(DocumentInfo));
        }
    }

    private void DeleteDocument(object sender, RoutedEventArgs e)
    {
        if (DocumentInfo == null)
        {
            throw new NullReferenceException(nameof(DocumentInfo));
        }

        if (gridContent.SelectedItem is not DocumentRefs refs || refs.FileName == null)
        {
            return;
        }

        if (MessageBox.Show($"Вы действительно хотите удалить документ?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
        {
            return;
        }

        try
        {
            using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                conn.Execute("delete from document_refs where id = :id", refs, transaction);
                transaction.Commit();

                if (!string.IsNullOrEmpty(refs.S3object))
                {
                    var minio = ServiceLocator.Context.GetService<IMinioClient>();
                    RemoveObject.Run(minio, DocumentRefs.GetBucketForEntity(DocumentInfo), refs.S3object).Wait();
                }

                if (gridContent.ItemsSource is IList<DocumentRefs> list)
                {
                    list.Remove(refs);
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ExceptionHelper.Message(ex), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void EditDocument(object sender, RoutedEventArgs e) => EditDocumentRefs();

    private void GridContent_MouseDoubleClick(object sender, MouseButtonEventArgs e) => EditDocumentRefs();

    private void ScanDocument(object sender, RoutedEventArgs e)
    {
        if (DocumentInfo == null)
        {
            MessageBox.Show("Для добавления сопутствующих документов необходимо сначала сохранить текущий", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var scanDialog = new ScannerWindow();
        if (scanDialog.Scan(out var images))
        {
            string file;
            if (scanDialog.ImageStore == FileExtension.Pdf)
            {
                file = PdfHelper.CreateDocument(images, PdfNamingStrategy.DateTime);
            }
            else
            {
                file = FileHelper.GetTempFileName("Scanned Images", PdfNamingStrategy.DateTime, scanDialog.ImageStore);
                images[0].SaveToFile(file, scanDialog.ImageStore);
            }

            var refDialog = new DocumentRefWindow();
            if (refDialog.Create(DocumentInfo.Id, file, out var refs))
            {
                AddDocumentRef(refs, refDialog.FileNameWithPath, refDialog.CreateThumbnailImage, DocumentRefs.GetBucketForEntity(DocumentInfo));
            }
        }
    }
}
