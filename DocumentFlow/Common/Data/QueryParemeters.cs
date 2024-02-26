//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;

namespace DocumentFlow.Common.Data;

public class QueryParemeters
{
    public bool FromOnly { get; init; } = false;
    public bool IncludeDocumentsInfo { get; init; } = true;
    public string? Table { get; init; }
    public string Alias { get; init; } = "t0";
    public QuantityInformation Quantity { get; init; } = QuantityInformation.Full;
    public string OwnerIdName { get; init; } = "owner_id";
    public static QueryParemeters Default { get; } = new();
}
