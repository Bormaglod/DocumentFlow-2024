//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Common.Data;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class EnumTypeAttribute : Attribute
{
    public EnumTypeAttribute(string name) => Name = name;
    public string Name { get; }
}
