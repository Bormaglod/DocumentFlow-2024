//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Repository;

public class AccountRepository : DirectoryRepository<Account>, IAccountRepository, ITransientLifetime
{
    public AccountRepository(IDatabase database) : base(database) { }

    public IReadOnlyList<Account> GetAccounts(Contractor contractor) 
    {
        using var conn = GetConnection();
        return GetSlim(conn, contractor); 
    }

    public IReadOnlyList<Account> GetAccounts(IDbConnection connection, Contractor contractor) => GetSlim(connection, contractor);
}
