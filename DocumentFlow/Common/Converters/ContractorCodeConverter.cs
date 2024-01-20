//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows.Data;

using DocumentFlow.Common.Enums;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.Common.Converters;

public class ContractorCodeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var code = parameter?.ToString()?.ToUpper();
        if (code == null)
        {
            return string.Empty;
        }

        string mask = string.Empty;
        if (values[0] is SubjectsCivilLow subject)
        {
            switch (subject)
            {
                case SubjectsCivilLow.Person:
                    mask = code switch
                    {
                        "INN" => "\\d{4} \\d{6} \\d{2}",
                        _ => string.Empty
                    };

                    break;
                case SubjectsCivilLow.LegalEntity:
                    if (values[1] is Okopf okopf)
                    {
                        mask = okopf.Code switch
                        {
                            "50102" => code switch
                            {
                                "INN" => "\\d{4} \\d{6} \\d{2}",
                                "OGRN" => "\\d{1} \\d{2} \\d{2} \\d{9} \\d{1}",
                                _ => string.Empty
                            },
                            _ => code switch
                            {
                                "INN" => "\\d{4} \\d{5} \\d{1}",
                                "OGRN" => "\\d{1} \\d{2} \\d{2} \\d{2} \\d{5} \\d{1}",
                                _ => string.Empty
                            },
                        };
                    }

                    break;
            }
        }

        return mask;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
