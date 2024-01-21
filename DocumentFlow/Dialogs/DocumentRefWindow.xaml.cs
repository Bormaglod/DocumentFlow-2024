//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для DocumentRefWindow.xaml
/// </summary>
public partial class DocumentRefWindow : Window
{
    private string fileNameWithPath = string.Empty;

    public DocumentRefWindow()
    {
        InitializeComponent();
    }

    public bool CreateThumbnailImage => checkPreview.IsChecked == true;

    public string FileNameWithPath => fileNameWithPath;

    public bool Create(Guid owner, [MaybeNullWhen(false)] out DocumentRefs document)
    {
        textFileName.Text = string.Empty;
        textNote.Text = string.Empty;
        checkPreview.IsChecked = false;

        textFileName.IsEnabled = true;
        buttonSelectFile.IsEnabled = true;

        if (ShowDialog() == true)
        {
            document = CreateDocumentRefs(owner);
            return true;
        }

        document = default;
        return false;
    }

    public bool Create(Guid owner, string fileName, [MaybeNullWhen(false)] out DocumentRefs document)
    {
        fileNameWithPath = fileName;

        textFileName.Text = Path.GetFileName(fileNameWithPath);
        textNote.Text = string.Empty;
        checkPreview.IsChecked = false;

        textFileName.IsEnabled = false;
        buttonSelectFile.IsEnabled = false;

        if (ShowDialog() == true)
        {
            document = CreateDocumentRefs(owner);
            return true;
        }

        document = default;
        return false;
    }

    public bool Edit(DocumentRefs refs)
    {
        fileNameWithPath = refs.FileName ?? string.Empty;

        textFileName.Text = refs.FileName;
        textNote.Text = refs.Note;
        checkPreview.IsChecked = refs.ThumbnailExist;

        textFileName.IsEnabled = false;
        buttonSelectFile.IsEnabled = false;
        checkPreview.IsEnabled = false;

        if (ShowDialog() == true)
        {
            refs.Note = textNote.Text;
            return true;
        }

        return false;
    }

    private DocumentRefs CreateDocumentRefs(Guid owner)
    {
        var fileInfo = new FileInfo(fileNameWithPath);
        return new DocumentRefs
        {
            OwnerId = owner,
            FileName = Path.GetFileName(fileNameWithPath),
            Note = textNote.Text,
            FileLength = fileInfo.Length
        };
    }

    private void AcceptClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(fileNameWithPath))
        {
            MessageBox.Show("Необходимо выбрать файл. Без файла - никак.....", "Файл", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        else
        {
            DialogResult = true;
        }
    }

    private void SelectFileClick(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFileDialog dlg = new()
        {
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|Pdf Files|*.pdf"
        };

        if (dlg.ShowDialog() == true) 
        {
            fileNameWithPath = dlg.FileName;
            textFileName.Text = Path.GetFileName(fileNameWithPath);
        }
    }
}
