//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using Microsoft.Xaml.Behaviors;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;

using System.Windows;

namespace DocumentFlow.Common.Triggers;

public class PriceMethodEndEditTrigger : TargetedTriggerAction<SfDataGrid>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is CurrentCellEndEditEventArgs args)
        {
            if (!Target.CurrentColumn.AllowEditing)
            {
                return;
            }

            var datarow = Target.RowGenerator.Items.FirstOrDefault(dr => dr.RowIndex == args.RowColumnIndex.RowIndex);
            if (datarow == null || datarow.RowData is not CalculationMaterial material)
            {
                return;
            }

            var repo = ServiceLocator.Context.GetService<ICalculationMaterialRepository>();

            if (!material.IsEqualProperties(nameof(CalculationMaterial.PriceSettingMethod)))
            {
                UpdateCalculationMaterial(repo, repo.SetPriceSettingMethod, material, datarow, args.RowColumnIndex.RowIndex);
            }
            else
            {
                if (!material.IsEqualProperties(nameof(CalculationMaterial.Price)))
                {
                    if (material.PriceSettingMethod != PriceSettingMethod.Manual)
                    {
                        material.CancelEdit();
                    }
                    else
                    {
                        UpdateCalculationMaterial(repo, repo.SetPrice, material, datarow, args.RowColumnIndex.RowIndex);
                    }
                }
            }

            material.EndEdit();
        }
    }

    private void UpdateCalculationMaterial(ICalculationMaterialRepository repo, Action<CalculationMaterial> action, CalculationMaterial material, DataRowBase dataRow, int rowIndex)
    {
        try
        {
            action(material);
            repo.RefreshPrices(material);

            dataRow.Element.DataContext = null;
            Target.UpdateDataRow(rowIndex);
        }
        catch (Exception e)
        {
            material.CancelEdit();
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
