//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models.Entities;

public class State
{
    [Key]
    public short Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string StateName { get; set; } = string.Empty;
    public string? Note { get; set;}

    public override bool Equals(object? obj)
    {
        return obj is State state && Id == state.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public override string ToString() => StateName;
}
