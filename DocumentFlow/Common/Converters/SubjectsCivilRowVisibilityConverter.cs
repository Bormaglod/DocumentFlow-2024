//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DocumentFlow.Common.Converters;

public class SubjectsCivilRowVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        SubjectsCivilLow? subjects = parameter?.ToString()?.ToUpper() switch
        {
            "PERSON" => SubjectsCivilLow.Person,
            "LEGALENTITY" => SubjectsCivilLow.LegalEntity,
            _ => null
        };

        SubjectsCivilLow? valueSubject = (SubjectsCivilLow?)value;

        return (valueSubject == subjects) ? new GridLength(1, GridUnitType.Auto) : new GridLength(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
