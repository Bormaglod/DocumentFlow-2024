//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Syncfusion.Data;

using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace DocumentFlow.Common;

public partial class ComplexSummaryAggregate : ISummaryAggregate
{
    [GeneratedRegex("^Employees\\[(\\d+)\\].(\\w+)$")]
    private static partial Regex EmpPropertiesRegex();

    public decimal ComplexSum { get; set; }

    Action<IEnumerable, string, PropertyDescriptor> ISummaryAggregate.CalculateAggregateFunc()
    {
        return (items, property, pd) =>
        {
            if (items is IEnumerable<OperationInfo> enumerableItems && pd.Name == "ComplexSum")
            {
                Match m = EmpPropertiesRegex().Match(property);
                if (m.Success && int.TryParse(m.Groups[1].Value, out int empIndex))
                {
                    ComplexSum = enumerableItems.Sum(x => empIndex < x.Employees.Count ? x.Employees[empIndex].Salary : 0);
                }
            }
        };
    }
}
