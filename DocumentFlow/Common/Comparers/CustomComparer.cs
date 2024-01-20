//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using Syncfusion.Data;

using System.ComponentModel;
using System.Reflection;

namespace DocumentFlow.Common.Comparers;

public class CustomComparer : IComparer<object>, ISortDirection
{
    private readonly PropertyInfo? property;

    public CustomComparer() { }

    public CustomComparer(Type entityType, string mappingName)
    {
        property = entityType.GetProperty(mappingName);
    }

    public int Compare(object? x, object? y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        int res;
        if (x is Directory parent_x && y is Directory parent_y && (parent_x.IsFolder != parent_y.IsFolder))
        {
            res = parent_x.IsFolder ? -1 : 1;
        }
        else
        {
            if (x is DocumentInfo && y is DocumentInfo && property != null)
            {
                res = CompareObject(property.GetValue(x), property.GetValue(y));
            }
            else if (x is Group grp_x && y is Group grp_y)
            {
                res = CompareObject(grp_x.Key, grp_y.Key);
            }
            else
            {
                res = CompareString(x, y);
            }
        }

        return SortDirection == ListSortDirection.Ascending ? res : -res;
    }

    public ListSortDirection SortDirection { get; set; }

    private static int CompareObject(object? x, object? y)
    {
        if (x is IComparable cmp_x && y is IComparable cmp_y)
        {
            return cmp_x.CompareTo(cmp_y);
        }
        else
        {
            return CompareString(x, y);
        }
    }

    private static int CompareString(object? x, object? y)
    {
        if (x == y) return 0;

        string? x_str = x?.ToString();
        if (x_str == null)
            return -1;

        string? y_str = y?.ToString();
        if (y_str == null)
            return 1;

        return x_str.CompareTo(y_str);
    }
}
