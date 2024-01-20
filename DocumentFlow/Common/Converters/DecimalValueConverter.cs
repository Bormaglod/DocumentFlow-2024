//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DocumentFlow.Common.Converters;

public class DecimalValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return string.Empty;
        }

        if (parameter != null)
        {
            switch (parameter.ToString()?.ToUpper())
            {
                case "BIK":
                    return ((decimal)value).ToString("00 00 00 000");
                case "BANK":
                    return ((decimal)value).ToString("000 00 000 0 00000000 000");
                case "ACCOUNT":
                    return ((decimal)value).ToString("000 00 000 0 0000 0000000");
                case "KPP":
                    return ((decimal)value).ToString("0000 00 000");
                case "OKPO":
                    return ((decimal)value).ToString("00 00000 0");
                case "INN":
                    return ((decimal)value).ToString().Length switch
                    {
                        10 => ((decimal)value).ToString("0000 00000 0"),
                        12 => ((decimal)value).ToString("0000 000000 00"),
                        _ => ((decimal)value).ToString(),
                    };
                case "OGRN":
                    return ((decimal)value).ToString().Length switch
                    {
                        13 => ((decimal)value).ToString("0 00 00 00 00000 0"),
                        15 => ((decimal)value).ToString("0 00 00 000000000 0"),
                        _ => ((decimal)value).ToString(),
                    };
            }
        }

        return ((decimal)value).ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}
