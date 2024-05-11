//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common;
using DocumentFlow.Models.Entities;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для DocumentWindow.xaml
/// </summary>
[INotifyPropertyChanged]
public partial class DocumentWindow : Window
{
    private IEnumerable<GridComboColumn>? columns;

    [ObservableProperty]
    private BaseDocument? selectedItem;

    [ObservableProperty]
    private object? itemsSource;

    public DocumentWindow()
    {
        InitializeComponent();
    }

    public IEnumerable<GridComboColumn>? Columns 
    { 
        get => columns;
        set
        {
            if (columns != value)
            {
                columns = value;
                if (columns == null)
                {
                    gridContent.Columns.Clear();
                }
                else
                {
                    foreach (var info in columns.OrderBy(x => x.Order))
                    {
                        GridColumn? column = null;
                        switch (info)
                        {
                            case TextGridComboColumn:
                                column = new GridTextColumn();
                                break;
                            case DateTimeGridComboColumn dateTimeColumnInfo:
                                column = new GridDateTimeColumn() { 
                                    Pattern = dateTimeColumnInfo.Pattern,
                                    CustomPattern = dateTimeColumnInfo.CustomPattern
                                };
                                break;
                            case CurrencyGridComboColumn:
                                column = new GridCurrencyColumn();
                                ((GridCurrencyColumn)column).CurrencyGroupSizes = new Int32Collection(new int[] { 3 });
                                break;
                        }

                        if (column == null)
                        {
                            continue;
                        }

                        column.MappingName = info.MappingName;
                        column.HeaderText = info.Header;
                        if (!double.IsNaN(info.Width))
                        {
                            column.Width = info.Width;
                        }
                        else
                        {
                            column.ColumnSizer = GridLengthUnitType.AutoWithLastColumnFill;
                        }

                        gridContent.Columns.Add(column);
                    }
                }
            }
        }
    }

    private void ButtonSelect_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedItem != null)
        {
            DialogResult = true;
        }
    }

    private void GridContent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedItem != null)
        {
            DialogResult = true;
        }
    }

    private void GridContent_Loaded(object sender, RoutedEventArgs e)
    {
        if (SelectedItem != null) 
        {
            var rowIndex = gridContent.ResolveToRowIndex(gridContent.SelectedItem);
            var columnIndex = gridContent.ResolveToStartColumnIndex();
            gridContent.ScrollInView(new RowColumnIndex(rowIndex, columnIndex));
        }
    }
}
