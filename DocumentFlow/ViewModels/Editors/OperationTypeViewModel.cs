//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Editors;

public partial class OperationTypeViewModel : DirectoryEditorViewModel<OperationType>, ISelfTransientLifetime
{
    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private decimal salary;

    protected override string GetStandardHeader() => "Вид операции";

    protected override void RaiseAfterLoadDocument(OperationType entity)
    {
        ParentId = entity.ParentId;
        ItemName = entity.ItemName;
        Salary = entity.Salary;
    }

    protected override void UpdateEntity(OperationType entity)
    {
        entity.ParentId = ParentId;
        entity.ItemName = ItemName;
        entity.Salary = Salary;
    }

    partial void OnItemNameChanged(string? value)
    {
        UpdateHeader(value ?? "?");
    }
}
