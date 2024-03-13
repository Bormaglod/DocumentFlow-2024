//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Repository;

public class OurAccountRepository : DirectoryRepository<OurAccount>, IOurAccountRepository, ITransientLifetime
{
    public OurAccountRepository(IDatabase database) : base(database) { }

    public IReadOnlyList<OurAccount> GetAccounts()
    {
        using var conn = GetConnection();
        return GetAccounts(conn);
    }

    public IReadOnlyList<OurAccount> GetAccounts(IDbConnection connection)
    {
        var org = connection.QuerySingle<Organization>("select id from organization where default_org");
        return GetSlim(connection, org);
    }
}