//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows.Data;

namespace DocumentFlow.Common.Converters;

public class NullToBoolConverter : IValueConverter
{
    private readonly string direction = "inverse";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag = value == null;

        if (parameter is string strParameter && string.Compare(direction, strParameter, true) == 0)
        {
            flag = !flag;
        }

        return flag;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
