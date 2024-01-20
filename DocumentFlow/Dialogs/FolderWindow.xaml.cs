//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Windows;

namespace DocumentFlow.Dialogs;

public delegate bool GroupOperation(string code, string name);

/// <summary>
/// Логика взаимодействия для GroupWindow.xaml
/// </summary>
public partial class FolderWindow : Window
{
    private readonly GroupOperation operation;

    public FolderWindow(GroupOperation addGroup)
    {
        InitializeComponent();

        operation = addGroup;
    }

    public FolderWindow(Directory directory, GroupOperation funcEdit) 
    {
        InitializeComponent();

        operation = funcEdit;
        textCode.Text = directory.Code;
        textName.Text = directory.ItemName;
    }

    private void AcceptClick(object sender, RoutedEventArgs e)
    {
        if (operation == null)
        {
            return;
        }

        if (operation(textCode.Text, textName.Text))
        {
            DialogResult = true;
        }
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        textCode.Focus();
    }
}
