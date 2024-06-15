//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common;
using DocumentFlow.Common.Enums;
using DocumentFlow.Models.Entities;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.TreeGrid;

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для DirectoryWindow.xaml
/// </summary>
[INotifyPropertyChanged]
public partial class DirectoryWindow : Window
{
    private IEnumerable<Common.GridComboColumn>? columns;

    [ObservableProperty]
    private Directory? selectedItem;

    [ObservableProperty]
    private object? itemsSource;

    public DirectoryWindow()
    {
        InitializeComponent();
    }

    public bool CanSelectFolder { get; set; }

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
                        TreeGridColumn? column = null;
                        switch (info)
                        {
                            case TextGridComboColumn:
                                column = new TreeGridTextColumn();
                                break;
                            case DateTimeGridComboColumn dateTimeColumnInfo:
                                column = new TreeGridDateTimeColumn() { Pattern = dateTimeColumnInfo.Pattern };
                                break;
                            case CurrencyGridComboColumn currencyColumnInfo:
                                column = new TreeGridCurrencyColumn()
                                {
                                    CurrencyGroupSizes = new Int32Collection([currencyColumnInfo.Grouping ? 3 : 0])
                                };

                                break;
                            case NumericGridComboColumn numericColumnInfo:
                                column = new TreeGridNumericColumn() 
                                { 
                                    NumberDecimalDigits = numericColumnInfo.NumberDecimalDigits,
                                    NumberGroupSizes = new Int32Collection([numericColumnInfo.Grouping ? 3 : 0])
                                };

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
                            if (info.ColumnSizer == GridColumnSizerType.None)
                            {
                                column.ColumnSizer = TreeColumnSizer.AutoFillColumn;
                            }
                            else
                            {
                                switch (info.ColumnSizer)
                                {
                                    case GridColumnSizerType.Auto:
                                        column.ColumnSizer = TreeColumnSizer.Auto;
                                        break;
                                    case GridColumnSizerType.AutoLastFill:
                                        column.ColumnSizer = TreeColumnSizer.AutoFillColumn;
                                        break;
                                    case GridColumnSizerType.SizeToCells:
                                        column.ColumnSizer = TreeColumnSizer.SizeToCells;
                                        break;
                                    case GridColumnSizerType.SizeToHeader:
                                        column.ColumnSizer = TreeColumnSizer.SizeToHeader;
                                        break;
                                    case GridColumnSizerType.Star:
                                        column.ColumnSizer = TreeColumnSizer.Star;
                                        break;
                                }
                            }
                        }

                        gridContent.Columns.Add(column);
                    }
                }
            }
        }
    }

    private void ButtonSelect_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedItem != null && (!SelectedItem.IsFolder || CanSelectFolder))
        {
            DialogResult = true;
        }
    }

    private void SfTreeGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedItem != null && (!SelectedItem.IsFolder || CanSelectFolder))
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
