//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows.Data;

using DocumentFlow.Common.Extensions;

namespace DocumentFlow.Common.Converters;

public class EnumConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return string.Empty;
        }

        foreach (Enum one in Enum.GetValues((Type)parameter))
        {
            if (value.Equals(one))
            {
                return one.Description();
            }
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null!;
        }

        foreach (Enum one in Enum.GetValues((Type)parameter))
        {
            if (value.ToString() == one.Description())
            {
                return one;
            }
        }

        return null!;
    }
}