//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.TreeGrid;

using System.Windows;
using System.Windows.Input;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для DirectoryWindow.xaml
/// </summary>
public partial class DirectoryWindow : Window
{
    private class ColumnInfo
    {
        public ColumnInfo(string mappingName, string header, double width)
        {
            MappingName = mappingName;
            Header = header;
            Width = width;
        }

        public string MappingName { get; set; }
        public string Header { get; set; }
        public double Width { get; set; }
    }

    private readonly IEnumerable<ColumnInfo> columns = new List<ColumnInfo>();

    public DirectoryWindow()
    {
        InitializeComponent();
    }

    public bool CanSelectFolder { get; set; }

    public Directory SelectedItem
    {
        get { return (Directory)GetValue(SelectedItemProperty); }
        set { SetValue(SelectedItemProperty, value); }
    }

    public IEnumerable<Directory> ItemsSource
    {
        get { return (IEnumerable<Directory>)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(Directory),
        typeof(DirectoryWindow));

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(IEnumerable<Directory>),
        typeof(DirectoryWindow));

    private void ButtonSelect_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedItem != null && (!SelectedItem.IsFolder || CanSelectFolder))
        {
            DialogResult = true;
        }
    }

    private void SfTreeGrid_AutoGeneratingColumn(object sender, TreeGridAutoGeneratingColumnEventArgs e)
    {
        var column = columns.FirstOrDefault(x => x.MappingName == e.Column.MappingName);
        if (column != null) 
        {
            e.Column.HeaderText = column.Header;
            if (!double.IsNaN(column.Width)) 
            { 
                e.Column.Width = column.Width;
            }
            else
            {
                e.Column.ColumnSizer = TreeColumnSizer.AutoFillColumn;
            }
        }
        else
        {
            e.Cancel = true;
        }
    }

    private void SfTreeGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!SelectedItem.IsFolder || CanSelectFolder)
        {
            DialogResult = true;
        }
    }

    private void GridContent_Loaded(object sender, RoutedEventArgs e)
    {
        var rowIndex = gridContent.ResolveToRowIndex(gridContent.SelectedItem);
        var columnIndex = gridContent.ResolveToStartColumnIndex();
        gridContent.ScrollInView(new RowColumnIndex(rowIndex, columnIndex));
    }
}
