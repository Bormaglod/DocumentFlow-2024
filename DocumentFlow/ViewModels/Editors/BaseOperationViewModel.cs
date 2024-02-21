//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Editors;

public abstract partial class BaseOperationViewModel<T> : DirectoryEditorViewModel<T>
    where T : Operation, new()
{
    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private int produced;

    [ObservableProperty]
    private int prodTime;

    [ObservableProperty]
    private int productionRate;

    [ObservableProperty]
    private DateTime? dateNorm;

    [ObservableProperty]
    private decimal salary;

    protected override void RaiseAfterLoadDocument(T entity)
    {
        Code = entity.Code;
        ParentId = entity.ParentId;
        ItemName = entity.ItemName;
        Produced = entity.Produced;
        ProdTime = entity.ProdTime;
        ProductionRate = entity.ProductionRate;
        DateNorm = entity.DateNorm;
        Salary = entity.Salary;
    }

    protected override void UpdateEntity(T entity)
    {
        entity.Code = Code;
        entity.ParentId = ParentId;
        entity.ItemName = ItemName;
        entity.Produced = Produced;
        entity.ProdTime = ProdTime;
        entity.ProductionRate = ProductionRate;
        entity.DateNorm = DateNorm;
        entity.Salary = Salary;
    }

    partial void OnItemNameChanged(string? value)
    {
        UpdateHeader(value ?? "?");
    }
}
