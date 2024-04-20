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

    /*[Display(AutoGenerateField = false)]
    public Guid ReferenceId 
    { 
        get => referenceId;
        set => SetProperty(ref referenceId, value);
    }

    [Display(Name = "Материал / Изделие", Order = 1)]
    [ColumnMode(AutoSizeColumnsMode = AutoSizeColumnsMode.Fill)]
    public string ProductName { get; protected set; } = string.Empty;

    [Display(Name = "Артикул", Order = 100)]
    [ColumnMode(Width = 150)]
    public string Code { get; protected set; } = string.Empty;

    [Display(Name = "Ед. изм.", Order = 150)]
    [ColumnMode(Width = 80, Alignment = HorizontalAlignment.Center)]
    public string MeasurementName { get; protected set; } = string.Empty;*/

    /*[Display(Name = "Количество", Order = 200)]
    [ColumnMode(Width = 100, Alignment = HorizontalAlignment.Right, DecimalDigits = 3)]
    public decimal Amount 
    { 
        get => amount;
        set => SetProperty(ref amount, value);
    */

    /*[Display(Name = "Цена", Order = 300)]
    [ColumnMode(Format = ColumnFormat.Currency, Width = 80)]
    public decimal Price 
    { 
        get => price;
        set => SetProperty(ref price, value);
    }

    [Display(Name = "Сумма", Order = 400)]
    [ColumnMode(Format = ColumnFormat.Currency, Width = 120)]
    public decimal ProductCost 
    { 
        get => productCost;
        set => SetProperty(ref productCost, value);
    }

    [Display(Name = "%НДС", Order = 500)]
    [ColumnMode(Width = 70, Alignment = HorizontalAlignment.Center)]
    public int Tax 
    { 
        get => tax;
        set => SetProperty(ref tax, value);
    }

    [Display(Name = "НДС", Order = 600)]
    [ColumnMode(Format = ColumnFormat.Currency, Width = 100)]
    public decimal TaxValue 
    { 
        get => taxValue;
        set => SetProperty(ref taxValue, value);
    }

    [Display(Name = "Всего с НДС", Order = 700)]
    [ColumnMode(Format = ColumnFormat.Currency, Width = 130)]
    public decimal FullCost 
    { 
        get => fullCost;
        set => SetProperty(ref fullCost, value);
    }*/

    /*public object Copy()
    {
        var copy = (ProductPrice)MemberwiseClone();
        copy.Id = 0;

        return copy;
    }

    public void CopyFrom(ProductPrice source) 
    {
        ReferenceId = source.ReferenceId;
        Amount = source.Amount;
        Price = source.Price;
        ProductCost = source.ProductCost;
        Tax = source.Tax;
        TaxValue = source.TaxValue;
        FullCost = source.FullCost;
        ProductName = source.ProductName;
        Code = source.Code;
        MeasurementName = source.MeasurementName;
    }

    public void SetProductInfo(Product product)
    {
        (Code, ProductName, MeasurementName) = (product.Code, product.ItemName ?? string.Empty, product.MeasurementName ?? string.Empty);
        if (this is IDiscriminator discriminator)
        {
            discriminator.Discriminator = product.GetType().Name.Underscore();
        }
    }

    public void SetOwner(Guid ownerId) => OwnerId = ownerId;*/
}
