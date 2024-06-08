//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using Humanizer;

using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public class ContractorRepository(IDatabase database) : 
    CompanyRepository<Contractor>(database), 
    IContractorRepository, 
    ITransientLifetime
{
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

    public IReadOnlyList<Contract> GetContracts(Contractor contractor, ContractorType? contractorType = null)
    {
        using var conn = GetConnection();
        return GetContracts(conn, contractor, contractorType);
    }

    public IReadOnlyList<Contract> GetContracts(IDbConnection connection, Contractor contractor, ContractorType? contractorType = null)
    {
        return connection.GetQuery<Contract>()
            .WhereFalse("t0.deleted")
            .Where("t0.owner_id", contractor.Id)
            .When(contractorType != null, q => q
                .WhereRaw($"t0.c_type = '{contractorType.ToString().Underscore()}'::contractor_type")
            )
            .OrderBy("t0.item_name")
            .Get<Contract>()
            .ToList();
    }

    public IReadOnlyList<Contractor> GetSuppliers()
    {
        using var conn = GetConnection();
        return GetSuppliers(conn);
    }

    public IReadOnlyList<Contractor> GetSuppliers(IDbConnection connection) => GetContractorByType(connection, ContractorType.Seller);

    public IReadOnlyList<Contractor> GetCustomers()
    {
        using var conn = GetConnection();
        return GetCustomers(conn);
    }

    public IReadOnlyList<Contractor> GetCustomers(IDbConnection connection) => GetContractorByType(connection, ContractorType.Buyer);

    private static List<Contractor> GetContractorByType(IDbConnection connection, ContractorType type)
    {
        return connection.GetQuery<Contractor>()
            .Distinct()
            .LeftJoin("contract as c", "c.owner_id", "t0.id")
            .WhereFalse("t0.deleted")
            .Where(q => q
                .WhereTrue("t0.is_folder")
                .OrWhereRaw($"c.c_type = '{type.ToString().Underscore()}'::contractor_type")
            )
            .Get<Contractor>()
            .ToList();
    }
}
