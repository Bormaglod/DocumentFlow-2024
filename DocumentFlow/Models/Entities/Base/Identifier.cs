//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models.Entities;

public abstract class Identifier : ObservableObject, IIdentifier
{
    [Key]
    public Guid Id { get; set; }

    public bool Identical(IIdentifier other)
    {
        return other is Identifier identifier && Id.Equals(identifier.Id);
    }
}
