//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Editors;

public partial class OkopfViewModel : DirectoryEditorViewModel<Okopf>, ISelfTransientLifetime
{
    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    public OkopfViewModel() { }

    public OkopfViewModel(IDatabase database) : base(database) { }

    protected override string GetStandardHeader() => "ОКОПФ";

    protected override void RaiseAfterLoadDocument(Okopf entity)
    {
        ParentId = entity.ParentId;
        Code = entity.Code;
        ItemName = entity.ItemName;
    }

    protected override void UpdateEntity(Okopf entity)
    {
        entity.ParentId = ParentId;
        entity.Code = Code;
        entity.ItemName = ItemName;
    }

    partial void OnItemNameChanged(string? value)
    {
        UpdateHeader(value ?? "?");
    }
}