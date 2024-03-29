//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models.Entities;

public abstract class Identifier : ObservableObject
{
    [Key]
    public Guid Id { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Identifier identifier && Id.Equals(identifier.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}
