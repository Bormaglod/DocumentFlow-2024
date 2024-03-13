//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Syncfusion.UI.Xaml.Grid;

using System.Windows;
using System.Windows.Controls;

namespace DocumentFlow.Common.Selectors;

public class TableSummaryStyleSelector : StyleSelector
{
    public override Style SelectStyle(object item, DependencyObject container)
    {
        if (container is FrameworkElement fe)
        {
            if (container is GridTableSummaryCell cell)
            {
                if (cell.ColumnBase.GridColumn is GridNumericColumn || cell.ColumnBase.GridColumn is GridCurrencyColumn)
                {
                    return (Style)fe.FindResource("TableSummaryStyleNumeric");
                }
            }

            return (Style)fe.FindResource("TableSummaryStyleDefault");
        }

        return null!;
    }
}
