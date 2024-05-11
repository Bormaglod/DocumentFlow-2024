//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models.Entities;

public class State : IIdentifier
{
    public const short Unknown = 0;
    public const short Compiled = 1000;
    public const short Canceled = 1001;
    public const short Completed = 1002;
    public const short InActive = 1003;

    [Key]
    public short Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string StateName { get; set; } = string.Empty;
    public string? Note { get; set;}

    public bool Identical(IIdentifier other) => Equals(other);

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
