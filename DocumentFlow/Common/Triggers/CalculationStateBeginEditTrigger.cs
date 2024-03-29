//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Xaml.Behaviors;

using Syncfusion.UI.Xaml.Grid;

namespace DocumentFlow.Common.Triggers;

public class CalculationStateBeginEditTrigger : TargetedTriggerAction<SfDataGrid>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is CurrentCellBeginEditEventArgs args)
        {
            if (Target.CurrentColumn.MappingName != nameof(Calculation.CalculationState))
            {
                return;
            }

            var datarow = Target.RowGenerator.Items.FirstOrDefault(dr => dr.RowIndex == args.RowColumnIndex.RowIndex);
            if (datarow?.RowData is Calculation calculation)
            {
                calculation.BeginEdit();
            }
        }
    }
}