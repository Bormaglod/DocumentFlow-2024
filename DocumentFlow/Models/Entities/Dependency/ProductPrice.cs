//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;
using DocumentFlow.Interfaces;

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models.Entities;

public abstract partial class ProductPrice : ObservableObject, IDependentEntity
{
    [ObservableProperty]
    [property: ForeignKey(FieldKey = "reference_id")]
    private Product? product;

    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private decimal price;

    [ObservableProperty]
    private decimal productCost;

    [ObservableProperty]
    private int tax;

    [ObservableProperty]
    private decimal taxValue;

    [ObservableProperty]
    private decimal fullCost;

    [Key]
    public long Id { get; set; }

    public Guid? OwnerId { get; set; }

    public void SetOwner(DocumentInfo owner) => OwnerId = owner.Id;
}
