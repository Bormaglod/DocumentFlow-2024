//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models.Entities;

public partial class CompatiblePart : ObservableObject, IDependentEntity
{
    [ObservableProperty]
    private Material? compatible;

    [Key]
    public long Id { get; set; }

    public Guid? OwnerId { get; set; }
}
