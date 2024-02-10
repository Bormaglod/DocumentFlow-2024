//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public abstract partial class ProductViewModel<T> : DirectoryEditorViewModel<T>
    where T : Product, new()
{
    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private decimal price;

    [ObservableProperty]
    private int vat;

    [ObservableProperty]
    private string? docName;

    [ObservableProperty]
    private Measurement? measurement;

    [ObservableProperty]
    private decimal? weight;

    [ObservableProperty]
    private IEnumerable<int> vats = new List<int>() { 0, 10, 20 };

    [ObservableProperty]
    private IEnumerable<Measurement>? measurements;

    public ProductViewModel() { }

    public ProductViewModel(IDatabase database) : base(database) { }

    protected override void RaiseAfterLoadDocument(T entity)
    {
        Code = entity.Code;
        ParentId = entity.ParentId;
        ItemName = entity.ItemName;
        Price = entity.Price;
        Vat = entity.Vat;
        DocName = entity.DocName;
        Weight = entity.Weight;
        Measurement = entity.Measurement;
    }

    protected override void UpdateEntity(T entity)
    {
        entity.Code = Code;
        entity.ParentId = ParentId;
        entity.ItemName = ItemName;
        entity.Price = Price;
        entity.Vat = Vat;
        entity.DocName = DocName;
        entity.Weight = Weight;
        entity.Measurement = Measurement;
    }

    protected override void InitializeEntityCollections(IDbConnection connection, T? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);
        
        Measurements = GetForeignData<Measurement>(connection);
    }

    partial void OnItemNameChanged(string? value)
    {
        UpdateHeader(value ?? "?");
    }
}