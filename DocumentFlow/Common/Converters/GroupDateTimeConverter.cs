//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;

using System.Globalization;
using System.Windows.Data;

namespace DocumentFlow.Common.Converters;

public class GroupDateTimeConverter : CustomGroupConverter, IValueConverter
{
    public GroupDateTimeConverter(string name) : base(name) { }

    public DateTimeGrouping Grouping { get; set; } = DateTimeGrouping.Default;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Grouping == DateTimeGrouping.Default)
        {
            return value;
        }

        if (PropertyReflector != null && ColumnName != null)
        {
            var dateGroupValue = PropertyReflector.GetValue(value, ColumnName);
            if (dateGroupValue is DateTime date)
            {
                switch (Grouping)
                {
                    case DateTimeGrouping.ByDate:
                        return DateOnly.FromDateTime(date);
                    case DateTimeGrouping.ByMonth:
                        return DateMonthOnly.FromDateTime(date);
                }
            }
        }

        return null!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
