//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Editors;

public partial class WireViewModel : DirectoryEditorViewModel<Wire>, ISelfTransientLifetime
{
    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private decimal wsize;

    public WireViewModel() { }

    public WireViewModel(IDatabase database) : base(database) { }

    protected override string GetStandardHeader() => "Тип провода";

    protected override void RaiseAfterLoadDocument(Wire entity)
    {
        ParentId = entity.ParentId;
        Code = entity.Code;
        ItemName = entity.ItemName;
        Wsize = entity.Wsize;
    }

    protected override void UpdateEntity(Wire entity)
    {
        entity.ParentId = ParentId;
        entity.Code = Code;
        entity.ItemName = ItemName;
        entity.Wsize = Wsize;
    }

    partial void OnItemNameChanged(string? value)
    {
        UpdateHeader(value ?? "?");
    }
}