//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models.Entities;

public class Property : Identifier
{
    public string PropertyName { get; set; } = string.Empty;
    public string? Title { get; set; }
    public override string ToString() => Title ?? PropertyName;
}
