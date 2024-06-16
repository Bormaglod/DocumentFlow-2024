//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Editors;

public partial class EquipmentViewModel : DirectoryEditorViewModel<Equipment>, ISelfTransientLifetime
{
    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private string? serialNumber;

    [ObservableProperty]
    private DateTime? commissioning;

    [ObservableProperty]
    private int? startingHits;

    [ObservableProperty]
    private bool isTools;

    protected override string GetStandardHeader() => "Оборудование";

    protected override void DoAfterLoadDocument(Equipment entity)
    {
        ParentId = entity.ParentId;
        Code = entity.Code;
        ItemName = entity.ItemName;
        SerialNumber = entity.SerialNumber;
        Commissioning = entity.Commissioning;
        StartingHits = entity.StartingHits;
        IsTools = entity.IsTools;
    }

    protected override void UpdateEntity(Equipment entity)
    {
        entity.ParentId = ParentId;
        entity.Code = Code;
        entity.ItemName = ItemName;
        entity.SerialNumber = SerialNumber;
        entity.Commissioning = Commissioning;
        entity.StartingHits = StartingHits;
        entity.IsTools = IsTools;
    }

    partial void OnItemNameChanged(string? value)
    {
        UpdateHeader(value ?? "?");
    }
}
