//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using Syncfusion.Data;

using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace DocumentFlow.Common;

public class DocumentInfoAggregate : ISummaryAggregate
{
    private readonly Dictionary<RuntimeTypeHandle, Dictionary<string, PropertyInfo>> typeProperties = new();

    public int Count { get; set; }
    public decimal Sum { get; set; }

    Action<IEnumerable, string, PropertyDescriptor> ISummaryAggregate.CalculateAggregateFunc()
    {
        return (items, property, pd) =>
        {
            if (items is IEnumerable<AccountingDocument> docItems)
            {
                switch (pd.Name)
                {
                    case "Count":
                        Count = docItems.Count(r => r.CarriedOut && !r.Deleted);
                        break;
                    case "Sum":
                        var first = docItems.FirstOrDefault();
                        if (first == null)
                        {
                            Sum = 0;
                        }
                        else
                        {
                            var p = TypePropertyCache(first.GetType(), property);
                            Sum = docItems
                                .Where(r => r.CarriedOut && !r.Deleted)
                                .Sum(x => Convert.ToDecimal(p.GetValue(x)));
                        }

                        break;
                }
            }
            else if (items is IEnumerable<DocumentInfo> enumerableItems)
            {
                switch (pd.Name)
                {
                    case "Count":
                        Count = enumerableItems.Count(r => !r.Deleted);
                        break;
                    case "Sum":
                        var first = enumerableItems.FirstOrDefault();
                        if (first == null)
                        {
                            Sum = 0;
                        }
                        else
                        {
                            var p = TypePropertyCache(first.GetType(), property);
                            Sum = p != null ? enumerableItems.Where(r => !r.Deleted).Sum(x => Convert.ToDecimal(p.GetValue(x))) : 0;
                        }

                        break;
                }
            }
        };
    }

    private PropertyInfo TypePropertyCache(Type type, string propertyName)
    {
        if (typeProperties.TryGetValue(type.TypeHandle, out Dictionary<string, PropertyInfo>? pis))
        {
            if (pis.TryGetValue(propertyName, out var prop))
            {
                return prop;
            }
        }

        pis ??= new Dictionary<string, PropertyInfo>();

        var p = type.GetProperty(propertyName) ?? throw new Exception($"Свойство {propertyName} не найдено в классе {type.Name}");
        pis.Add(propertyName, p);

        return p;
    }
}
