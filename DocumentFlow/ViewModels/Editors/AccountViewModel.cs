﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public partial class AccountViewModel : DirectoryEditorViewModel<Account>, ISelfTransientLifetime
{
    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private decimal account;

    [ObservableProperty]
    private Bank? bank;

    [ObservableProperty]
    private IEnumerable<Bank>? banks;

    protected override string GetStandardHeader() => "Расч. счёт";

    protected override void RaiseAfterLoadDocument(Account entity)
    {
        ParentId = entity.ParentId;
        ItemName = entity.ItemName;
        Account = entity.AccountValue;
        Bank = entity.Bank;
    }

    protected override void UpdateEntity(Account entity)
    {
        entity.ParentId = ParentId;
        entity.ItemName = ItemName;
        entity.AccountValue = Account;
        entity.Bank = Bank;
    }

    protected override void InitializeEntityCollections(IDbConnection connection, Account? account)
    {
        base.InitializeEntityCollections(connection, account);
        Banks = GetForeignDirectory<Bank>(connection);
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<Account>(x => x.Bank);
    }

    protected override void Load()
    {
        Load<Bank>((account, bank) =>
        {
            account.Bank = bank;
            return account;
        });
    }

    partial void OnItemNameChanged(string? value)
    {
        UpdateHeader(value ?? "?");
    }
}