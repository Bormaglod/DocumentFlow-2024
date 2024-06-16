//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public partial class AccountViewModel : DirectoryEditorViewModel<Account>, ISelfTransientLifetime
{
    private readonly IBankRepository bankRepository = null!;

    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private decimal account;

    [ObservableProperty]
    private Bank? bank;

    [ObservableProperty]
    private IEnumerable<Bank>? banks;

    public AccountViewModel() { }

    public AccountViewModel(IBankRepository bankRepository) : base()
    {
        this.bankRepository = bankRepository;
    }

    protected override string GetStandardHeader() => "Расч. счёт";

    protected override void DoAfterLoadDocument(Account entity)
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
        Banks = bankRepository.GetSlim(connection);
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<Account>(x => x.Bank);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<Bank>(connection , (account, bank) =>
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