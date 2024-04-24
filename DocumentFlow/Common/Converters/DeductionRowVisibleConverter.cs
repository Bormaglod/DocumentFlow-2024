//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DocumentFlow.Common.Converters;

public class DeductionRowVisibleConverter : IValueConverter
{
    private readonly string direction = "inverse";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Deduction deduction)
        {
            var flag = deduction.BaseDeduction != Enums.BaseDeduction.Person;
            if (parameter is string strParameter && string.Compare(direction, strParameter, true) == 0)
            {
                flag = !flag; 
            }

            return flag ? new GridLength(0) : new GridLength(1, GridUnitType.Auto);
        }

        return new GridLength(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
