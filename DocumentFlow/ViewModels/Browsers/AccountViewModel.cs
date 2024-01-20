//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public class AccountViewModel : DirectoryViewModel<Account>, ISelfTransientLifetime
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

    protected override IReadOnlyList<Account> GetData(IDbConnection connection, Guid? id)
    {
        return GetData<Bank, Contractor>(
            connection,
            (account, bank, contractor) => 
            { 
                account.Bank = bank; 
                account.Company = contractor;
                return account; 
            },
            (refs) =>
            {
                if (refs == "contractor_id")
                {
                    return "owner_id";
                }

                return refs;
            },
            id);
    }
}
