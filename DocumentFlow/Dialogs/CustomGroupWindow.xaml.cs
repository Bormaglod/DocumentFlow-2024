﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DocumentFlow.Common;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для CustomGroupWindow.xaml
/// </summary>
[INotifyPropertyChanged]
public partial class CustomGroupWindow : Window
{
    [ObservableProperty]
    private CustomGroupColumn? availableColumn;

    [ObservableProperty]
    private CustomGroupColumn? selectedColumn;

    public CustomGroupWindow(IEnumerable<CustomGroupColumn> availables, IEnumerable<CustomGroupColumn> selected)
    {
        InitializeComponent();

        foreach (var item in availables.Except(selected))
        {
            Availables.Add(item);
        }

        foreach (var item in selected.OrderBy(x => x.Order))
        {
            Selected.Add(item);
        }
    }

    public ObservableCollection<CustomGroupColumn> Availables { get; } = new();

    public ObservableCollection<CustomGroupColumn> Selected { get; } = new();

    [RelayCommand]
    private void SelectColumn()
    {
        if (AvailableColumn != null)
        {
            var sel = Selected.FirstOrDefault(x => x.MappingName == AvailableColumn.MappingName);
            if (sel != null)
            {
                Selected.Remove(sel);
                Availables.Add(sel);
            }

            Selected.Add(AvailableColumn);
            Availables.Remove(AvailableColumn);
        }
    }

    [RelayCommand]
    private void DeselectColumn()
    {
        if (SelectedColumn != null)
        {
            Availables.Add(SelectedColumn);
            Selected.Remove(SelectedColumn);
        }
    }

    private void AcceptClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void ButtonSelect_Click(object sender, RoutedEventArgs e) => SelectColumn();

    private void ButtonDeselect_Click(object sender, RoutedEventArgs e) => DeselectColumn();

    private void ButtonToUp_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedColumn != null && Selected.Count > 1)
        {
            var cur = SelectedColumn;
            var prev = Selected.Where(x => x.Order < cur.Order).OrderBy(x => x.Order).LastOrDefault();

            if (prev != null)
            {
                (prev.Order, cur.Order) = (cur.Order, prev.Order);

                ICollectionView view = CollectionViewSource.GetDefaultView(listSelectedItems.ItemsSource);
                view.Refresh();
            }
        }
    }

    private void ButtonToBottom_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedColumn != null && Selected.Count > 1)
        {
            var cur = SelectedColumn;
            var next = Selected.Where(x => x.Order > cur.Order).OrderBy(x => x.Order).FirstOrDefault();

            if (next != null)
            {
                (next.Order, cur.Order) = (cur.Order, next.Order);

                ICollectionView view = CollectionViewSource.GetDefaultView(listSelectedItems.ItemsSource);
                view.Refresh();
            }
        }
    }
}
