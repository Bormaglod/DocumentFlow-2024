//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;

using Syncfusion.UI.Xaml.Grid;

using System.Windows.Controls;

namespace DocumentFlow.Views.Browsers;

/// <summary>
/// Логика взаимодействия для EquipmentView.xaml
/// </summary>
public partial class EquipmentView : UserControl, IGridPageView, ISelfTransientLifetime
{
    private readonly GridRowSizingOptions gridRowResizingOptions = new();
    public EquipmentView()
    {
        InitializeComponent();
    }

    public SfDataGrid DataGrid => gridContent;

    private void GridContent_QueryRowHeight(object sender, QueryRowHeightEventArgs e)
    {
        if (gridContent.GetHeaderIndex() == e.RowIndex)
        {
            if (gridContent.GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out var autoHeight))
            {
                if (autoHeight > 24)
                {
                    e.Height = autoHeight;
                    e.Handled = true;
                }
            }
        }
    }
}
