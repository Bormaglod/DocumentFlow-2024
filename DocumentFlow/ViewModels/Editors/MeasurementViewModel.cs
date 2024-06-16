//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Editors;

public partial class MeasurementViewModel : DirectoryEditorViewModel<Measurement>, ISelfTransientLifetime
{
    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private string? abbreviation;

    protected override string GetStandardHeader() => "Ед. изм.";

    protected override void DoAfterLoadDocument(Measurement entity)
    {
        ParentId = entity.ParentId;
        Code = entity.Code;
        ItemName = entity.ItemName;
        Abbreviation = entity.Abbreviation;
    }

    protected override void UpdateEntity(Measurement entity)
    {
        entity.ParentId = ParentId;
        entity.Code = Code;
        entity.ItemName = ItemName;
        entity.Abbreviation = Abbreviation;
    }

    partial void OnAbbreviationChanged(string? value)
    {
        UpdateHeader(value ?? "?");
    }
}