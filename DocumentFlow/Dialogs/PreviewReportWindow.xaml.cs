//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.ViewModels.Dialogs;

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для PreviewReportWindow.xaml
/// </summary>
public partial class PreviewReportWindow : Window
{
    public PreviewReportWindow()
    {
        InitializeComponent();

        AddHandler(LoadedEvent, new RoutedEventHandler(ControlIsLoaded));

        if (DataContext is PreviewReportViewModel model)
        {
            model.SetViewer(pdfViewer);
        }
    }

    private void HideVerticalToolbar()
    {
        // Hides the thumbnail icon. 
        pdfViewer.ThumbnailSettings.IsVisible = false;
        // Hides the bookmark icon. 
        pdfViewer.IsBookmarkEnabled = false;
        // Hides the layer icon. 
        pdfViewer.EnableLayers = false;
        // Hides the organize page icon. 
        pdfViewer.PageOrganizerSettings.IsIconVisible = false;
        // Hides the redaction icon. 
        pdfViewer.EnableRedactionTool = false;
        // Hides the form icon. 
        pdfViewer.FormSettings.IsIconVisible = false;
    }

    private int GetCurrentPageIndex()
    {
        if (string.IsNullOrEmpty(textCurrentPage.Text))
        {
            return pdfViewer.IsLoaded ? pdfViewer.CurrentPage : 1;
        }

        if (!int.TryParse(textCurrentPage.Text, out int result))
        {
            return -1;
        }

        return result;
    }

    private void ControlIsLoaded(object sender, RoutedEventArgs e)
    {
        HideVerticalToolbar();
    }

    private void PdfViewer_CurrentPageChanged(object sender, EventArgs args)
    {
        textCurrentPage.Text = pdfViewer.CurrentPage.ToString();
    }

    private void TextCurrentPage_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || pdfViewer.LoadedDocument == null)
        {
            return;
        }

        if (PageNumberRegex().IsMatch(textCurrentPage.Text))
        {
            pdfViewer.CurrentPage = GetCurrentPageIndex();
        }

        textCurrentPage.Text = pdfViewer.CurrentPage.ToString();
   }

    [GeneratedRegex("^[0-9]*$")]
    private static partial Regex PageNumberRegex();
}
