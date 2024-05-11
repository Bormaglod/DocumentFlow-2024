//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Windows.Data;

namespace DocumentFlow.Interfaces;

public interface ICustomGroupingView
{
    IReadOnlyList<(string MappingName, IValueConverter Converter)> GroupingColumns { get; }
}