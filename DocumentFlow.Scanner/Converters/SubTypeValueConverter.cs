//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows.Data;

using WIA;

namespace DocumentFlow.Scanner.Converters;

internal class SubTypeValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ScannerProperty prop)
        {
            if (prop.SubType == WiaSubType.RangeSubType)
            {
                return parameter?.ToString()?.ToUpper() switch
                {
                    "MIN" => prop.SubTypeMin,
                    "MAX" => prop.SubTypeMax,
                    "STEP" => prop.SubTypeStep,
                    _ => throw new Exception("The parameter value is not set or is not defined.")
                };
            }
        }

        return null!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
