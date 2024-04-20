//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace DocumentFlow.Common.Converters;

public partial class IntToPercentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int percent)
        {
            return $"{percent} %";
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var text = value?.ToString();
        if (string.IsNullOrEmpty(text)) 
        { 
            throw new NotSupportedException();
        }

        var match = PercentRegex().Match(text);
        if (match.Success)
        {
            return int.Parse(match.Groups[1].Value);
        }

        throw new FormatException();
    }

    [GeneratedRegex("(\\d+)\\s*%?$")]
    private static partial Regex PercentRegex();
}
