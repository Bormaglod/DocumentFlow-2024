//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Common.Data;

public class QueryParemeters
{
    public bool FromOnly { get; init; } = false;
    public bool IncludeDocumentsInfo { get; init; } = true;
    public static QueryParemeters Default { get; } = new();
}
