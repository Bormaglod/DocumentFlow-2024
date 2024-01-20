//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DocumentFlow.Common.Converters;

public class GridLengthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double val = (double)value;
        GridLength gridLength = new(val);

        return gridLength;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        GridLength val = (GridLength)value;

        return val.Value;
    }
}
