//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models.Entities;

public class Report : Identifier
{
    public static readonly Guid PurchaseRequest = new("d0edcb83-c298-4d19-a216-309d33687e40");
    public static readonly Guid Specification = new("19bc3ef4-5d0c-4d18-85e6-4e0a5f8f4522");
    public static readonly Guid ProcessMap = new("b7945033-3dd6-4d80-b809-db8fe8939a1a");

    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? SchemaReport { get; set; }
}