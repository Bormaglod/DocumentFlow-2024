//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Editors;

public partial class BankViewModel : DirectoryEditorViewModel<Bank>, ISelfTransientLifetime
{
    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private decimal bik;

    [ObservableProperty]
    private decimal account;

    [ObservableProperty]
    private string? town;

    protected override string GetStandardHeader() => "Банк";

    protected override void DoAfterLoadDocument(Bank entity)
    {
        ParentId = entity.ParentId;
        ItemName = entity.ItemName;
        Bik = entity.Bik;
        Account = entity.Account;
        Town = entity.Town;
    }

    protected override void UpdateEntity(Bank entity)
    {
        entity.ParentId = ParentId;
        entity.ItemName = ItemName;
        entity.Bik = Bik;
        entity.Account = Account;
        entity.Town = Town;
    }

    partial void OnItemNameChanged(string? value)
    {
        UpdateHeader(value ?? "?");
    }
}