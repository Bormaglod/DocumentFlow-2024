//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Data.Models;

public partial class PriceApproval: ObservableObject, IDependentEntity
{
    [ObservableProperty]
    private Product? product;

    [ObservableProperty]
    private decimal price;

    [Key]
    public long Id { get; set; }

    public Guid? OwnerId { get; set; }

    public void SetOwner(DocumentInfo owner) => OwnerId = owner.Id;
}