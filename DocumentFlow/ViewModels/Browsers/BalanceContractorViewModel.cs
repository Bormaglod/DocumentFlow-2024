//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class BalanceContractorViewModel : BalanceViewModel<BalanceContractor>, ISelfTransientLifetime
{
    public BalanceContractorViewModel() { }

    public BalanceContractorViewModel(IDatabase database, IConfiguration configuration, ILogger<BalanceContractorViewModel> logger) : base(database, configuration, logger) { }

    protected override IReadOnlyList<BalanceContractor> GetData(IDbConnection connection, Guid? id = null)
    {
        return connection.GetQuery<BalanceContractor>()
            .SelectRaw("case when amount > 0 then operation_summa else null end as contractor_debt")
            .SelectRaw("case when amount < 0 then operation_summa else null end as organization_debt")
            .SelectRaw("sum(operation_summa * sign(amount)) over (order by t0.document_date, t0.document_number) as debt")
            .Select("dt.code as document_code")
            .Select("dt.document_name as document_name")
            .Join("document_type as dt", "dt.id", "document_type_id")
            .MappingQuery<BalanceContractor>(x => x.Contract)
            .Where("reference_id", Owner?.Id)
            .Get<BalanceContractor, Contract>(
                map: (balance, contract) =>
                {
                    balance.Contract = contract;
                    return balance;
                })
            .ToList();
    }
}
