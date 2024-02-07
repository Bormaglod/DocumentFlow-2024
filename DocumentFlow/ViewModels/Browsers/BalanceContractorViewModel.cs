//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public class BalanceContractorViewModel : BalanceViewModel<BalanceContractor>, ISelfTransientLifetime
{
    public BalanceContractorViewModel() { }

    public BalanceContractorViewModel(IDatabase database) : base(database) { }

    protected override IReadOnlyList<BalanceContractor> GetData(IDbConnection connection, Guid? id = null)
    {
        return GetQuery(connection)
            .Select("t0.*")
            .SelectRaw("case when amount > 0 then operation_summa else null end as contractor_debt")
            .SelectRaw("case when amount < 0 then operation_summa else null end as organization_debt")
            .SelectRaw("sum(operation_summa * sign(amount)) over (order by t0.document_date, t0.document_number) as debt")
            .Select("dt.code as document_code")
            .Select("dt.document_name as document_name")
            .Select("c.{id, code, item_name, document_date}")
            .Join("document_type as dt", "dt.id", "document_type_id")
            .LeftJoin("contract as c", "c.id", "t0.contract_id")
            .Where("reference_id", Owner?.Id)
            .Get<BalanceContractor, Contract>()
            .ToList();
    }
}
