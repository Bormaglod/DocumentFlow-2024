//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using Microsoft.Xaml.Behaviors;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;

using System.Windows;

namespace DocumentFlow.Common.Triggers;

public class CalculationStateEndEditTrigger : TargetedTriggerAction<SfDataGrid>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is CurrentCellEndEditEventArgs args)
        {
            if (Target.CurrentColumn.MappingName != nameof(Calculation.CalculationState))
            {
                return;
            }

            var datarow = Target.RowGenerator.Items.FirstOrDefault(dr => dr.RowIndex == args.RowColumnIndex.RowIndex);
            if (datarow == null || datarow.RowData is not Calculation calculation)
            {
                return;
            }

            if (!calculation.IsEqualProperties(nameof(Calculation.CalculationState)))
            {
                try
                {
                    ServiceLocator.Context.GetService<ICalculationRepository>().SetState(calculation);

                    datarow.Element.DataContext = null;
                    Target.UpdateDataRow(args.RowColumnIndex.RowIndex);
                }
                catch (Exception e)
                {
                    calculation.CancelEdit();
                    MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            calculation.EndEdit();
        }
    }
}
