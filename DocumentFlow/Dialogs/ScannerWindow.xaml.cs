//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;
using DocumentFlow.ViewModels.Dialogs;

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для ScannerWindow.xaml
/// </summary>
public partial class ScannerWindow : Window
{
    public ScannerWindow()
    {
        InitializeComponent();

        Closing += ((ScannerViewModel)DataContext).OnWindowClosing;
    }

    public FileExtension ImageStore
    {
        get => ((ScannerViewModel)DataContext).ImageStore.ImageStore;
    }

    public bool Scan([MaybeNullWhen(false)] out IList<BitmapSource> images)
    {
        if (ShowDialog() == true && DataContext is ScannerViewModel model && model.Images.Count > 0)
        {
            images = model.Images;
            return true;
        }

        images = null;
        return false;
    }

    private void ButtonSave_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ScannerViewModel model)
        {
            if (model.Images.Count > 1 && model.ImageStore.ImageStore != FileExtension.Pdf)
            {
                MessageBox.Show("Для сохранения несколько изображений выберите тип сохранения PDF.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        DialogResult = true;
    }
}
