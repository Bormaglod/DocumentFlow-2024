﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public abstract class BalanceProductViewModel<T> : BalanceViewModel<T>
    where T : BalanceProduct
{
    public BalanceProductViewModel() { }

    public BalanceProductViewModel(IDatabase database, IConfiguration configuration, ILogger<BalanceProductViewModel<T>> logger) : base(database, configuration, logger) { }

    protected override IReadOnlyList<T> GetData(IDbConnection connection, Guid? id = null)
    {
        return connection.GetQuery<T>()
            .SelectRaw("case when amount > 0 then amount else null end as income")
            .SelectRaw("case when amount < 0 then @amount else null end as expense")
            .SelectRaw("sum(amount) over (order by document_date, document_number) as remainder")
            .SelectRaw("sum(operation_summa * sign(amount)) over (order by document_date, document_number) as monetary_balance")
            .Select("dt.code as document_code")
            .Select("dt.document_name as document_name")
            .Join("document_type as dt", "dt.id", "document_type_id")
            .Where("reference_id", Owner?.Id)
            .Get<T>()
            .ToList();
    }
}
