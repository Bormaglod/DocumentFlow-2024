//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public class ContractorRepository : CompanyRepository<Contractor>, IContractorRepository, ITransientLifetime
{
    public ContractorRepository(IDatabase database) : base(database) { }

    public IReadOnlyList<Account> GetAccounts(Contractor contractor)
    {
        using var conn = GetConnection();
        return GetAccounts(conn, contractor);
    }

    public IReadOnlyList<Account> GetAccounts(IDbConnection connection, Contractor contractor)
    {
        return GetCustomSlimQuery<Account>(connection, contractor)
            .Get<Account>()
            .ToList();
    }

    public IReadOnlyList<Contract> GetContracts(Contractor contractor)
    {
        using var conn = GetConnection();
        return GetContracts(conn, contractor);
    }

    public IReadOnlyList<Contract> GetContracts(IDbConnection connection, Contractor contractor)
    {
        return GetCustomSlimQuery<Contract>(connection, contractor)
            .Get<Contract>()
            .ToList();
    }
}
