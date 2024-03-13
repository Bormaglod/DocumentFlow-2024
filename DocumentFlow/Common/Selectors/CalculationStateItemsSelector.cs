//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Models.Entities;

using Syncfusion.UI.Xaml.Grid;

using System.Collections;
using System.Collections.ObjectModel;

namespace DocumentFlow.Common.Selectors;

public class CalculationStateItemsSelector : IItemsSourceSelector
{
    public IEnumerable GetItemsSource(object record, object dataContext)
    {
        if (record == null || record is not Calculation calculation)
        {
            return null!;
        }

        ObservableCollection<string> states = new()
        {
            calculation.CalculationState.Description(),
            calculation.CalculationState switch
            {
                CalculationState.Prepare => CalculationState.Approved.Description(),
                CalculationState.Approved => CalculationState.Expired.Description(),
                CalculationState.Expired => CalculationState.Prepare.Description(),
                _ => throw new NotSupportedException()
            }
        };

        return states;
    }
}