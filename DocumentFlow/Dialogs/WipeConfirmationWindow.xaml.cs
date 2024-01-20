//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;

using System.Windows;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для WipeConfirmationWindow.xaml
/// </summary>
public partial class WipeConfirmationWindow : Window
{
    public WipeConfirmationWindow()
    {
        InitializeComponent();
    }

    public WipeAction Action { get; set; } = WipeAction.Cancel;

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        onlyCurrentButton.Focus();
    }

    private void OnlyCurrentButton_Click(object sender, RoutedEventArgs e)
    {
        Action = WipeAction.Current;
        DialogResult = true;
    }

    private void AllButton_Click(object sender, RoutedEventArgs e)
    {
        Action = WipeAction.All;
        DialogResult = true;
    }
}
