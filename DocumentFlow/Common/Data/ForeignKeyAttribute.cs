//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Common.Data;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ForeignKeyAttribute : Attribute
{
    public ForeignKeyAttribute() { }

    public string? Name { get; set; }
    public string? ForeignFieldKey { get; set; }
    public string? ForeignFieldName { get; set; }
}