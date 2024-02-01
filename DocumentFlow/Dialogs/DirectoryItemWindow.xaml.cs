//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Controls;
using DocumentFlow.Models.Entities;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для DirectoryItemWindow.xaml
/// </summary>
public partial class DirectoryItemWindow : Window, INotifyPropertyChanged
{
    private Directory? selectedItem;
    private IEnumerable<Directory>? itemsSource;

    public event PropertyChangedEventHandler? PropertyChanged;

    public DirectoryItemWindow()
    {
        InitializeComponent();
    }

    public Directory? SelectedItem
    {
        get => selectedItem;
        set 
        { 
            if (selectedItem != value)
            { 
                selectedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
            }
        }
    }

    public bool Get<T>(IEnumerable<T> items, T? selected, [MaybeNullWhen(false)] out T directory)
        where T : Directory
    {
        itemsSource = items;
        SelectedItem = selected;
        if (ShowDialog() == true && SelectedItem is T item) 
        {
            directory = item;
            return true;
        }

        directory = null;
        return false;
    }

    private void ButtonSelect_Click(object sender, RoutedEventArgs e)
    {
        if (itemsSource == null) 
        {
            return;
        }

        DirectoryWindow window = new()
        {
            ItemsSource = itemsSource
        };

        if (SelectedItem != null)
        {
            var item = itemsSource.FirstOrDefault(x => x.Id == SelectedItem.Id);
            if (item != null) 
            {
                window.SelectedItem = item;
            }
        }

        if (window.ShowDialog() == true)
        {
            SelectedItem = window.SelectedItem;

        }
    }

    private void ButtonClear_Click(object sender, RoutedEventArgs e)
    {
        SelectedItem = null;
    }

    private void ButtonAccept_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedItem == null)
        {
            MessageBox.Show("Необходимо выбрать элемент справочника.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else 
        {
            DialogResult = true;
        }
    }
}
