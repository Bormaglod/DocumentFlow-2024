//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Humanizer;

using System.Globalization;

namespace DocumentFlow.Common;

public readonly struct DateMonthOnly : IComparable, IComparable<DateMonthOnly>
{
    private readonly int month;
    private readonly int year;

    private DateMonthOnly(DateTime date)
    {
        month = date.Month;
        year = date.Year;
    }

    public static DateMonthOnly FromDateTime(DateTime date)
    {
        return new DateMonthOnly(date);
    }

    public int Month => month;

    public int Year => year;

    public static DateMonthOnly MinValue => new DateMonthOnly(DateTime.MinValue);

    public static int Compare(DateMonthOnly t1, DateMonthOnly t2)
    {
        return t1.Year == t2.Year ? 
            t1.Month.CompareTo(t2.Month) : 
            t1.Year.CompareTo(t2.Year);
    }

    public int CompareTo(object? value)
    {
        if (value == null) return 1;
        if (value is not DateMonthOnly)
        {
            throw new ArgumentException("Аргумент должен быть типа DateMonthOnly");
        }

        return Compare(this, (DateMonthOnly)value);
    }

    public int CompareTo(DateMonthOnly value)
    {
        return Compare(this, value);
    }

    public override readonly string ToString()
    {
        return $"{CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[month - 1].Titleize()} {year} г.";
    }
}
