//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class AccountViewModel : DirectoryViewModel<Account>, ISelfTransientLifetime
{
    public AccountViewModel() { }

    public AccountViewModel(IDatabase database) : base(database) { }

    public override Type? GetEditorViewType() => typeof(Views.Editors.AccountView);

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.ItemName))
        {
            columnInfo.AlwaysVisible = true;
        }
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<Account>(x => x.Bank)
            .MappingQuery<Account>(x => x.Company);
    }

    protected override IReadOnlyList<Account> GetData(IDbConnection connection, Guid? id)
    {
        return DefaultQuery(connection, id)
            .Get<Account, Bank, Contractor>(
                map: (account, bank, contractor) =>
                {
                    account.Bank = bank;
                    account.Company = contractor;
                    return account;
                })
            .ToList();
    }
}
