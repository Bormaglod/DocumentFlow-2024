//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public class ContractRepository(IDatabase database) : 
    DirectoryRepository<Contract>(database), 
    IContractRepository, 
    ITransientLifetime
{
    public IReadOnlyList<ContractApplication> GetCurrentAnnexes(Contract contract, DateTime? actualDate = null)
    {
        using var conn = GetConnection();
        return GetCurrentAnnexes(conn, contract, actualDate);
    }

    public IReadOnlyList<ContractApplication> GetCurrentAnnexes(IDbConnection connection, Contract contract, DateTime? actualDate = null)
    {
        actualDate ??= DateTime.Now;
        return GetCustomSlimQuery<ContractApplication>(connection, contract)
            .WhereDate("t0.date_start", "<=", DateTime.Now)
            .Where(q => q
                .WhereNull("t0.date_end")
                .Or()
                .WhereDate("t0.date_end", ">=", DateTime.Now))
            .Get<ContractApplication>()
            .ToList();
    }

    public decimal GetPrice(Contract contract, Product product, DateTime? actualDate = null)
    {
        using var conn = GetConnection();
        return GetPrice(conn, contract, product, actualDate);
    }

    public decimal GetPrice(IDbConnection connection, Contract contract, Product product, DateTime? actualDate = null)
    {
        var parameters = new QueryParameters()
        {
            Quantity = QuantityInformation.None,
            Table = "price_approval",
            Alias = "pa"
        };

        actualDate ??= DateTime.Now;

        return connection.GetQuery<decimal>(parameters)
            .Select("pa.price")
            .Join("contract_application as ca", "ca.id", "pa.owner_id")
            .Where("ca.owner_id", contract.Id)
            .Where("pa.product_id", product.Id)
            .Where(q => q
                .Where(q => q
                    .Where("ca.date_start", "<=", actualDate)
                    .WhereNull("ca.date_end")
                )
                .Or()
                .Where(q => q
                    .Where("ca.date_start", "<=", actualDate)
                    .Where("ca.date_end", ">=", actualDate)
                )
            )
            .OrderByDesc("ca.date_start")
            .FirstOrDefault<decimal>();
    }
}
