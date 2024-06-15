//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public class WaybillSaleViewModel : WaybillViewModel<WaybillSale>, ISelfTransientLifetime
{
    private readonly IWaybillSaleRepository waybillRepository = null!;

    public WaybillSaleViewModel() { }

    public WaybillSaleViewModel(IDatabase database, IWaybillSaleRepository waybillRepository, IConfiguration configuration, ILogger<WaybillSaleViewModel> logger)
        : base(database, configuration, logger)
    {
        this.waybillRepository = waybillRepository;
    }

    public override Type? GetEditorViewType() => typeof(Views.Editors.WaybillSaleView);

    protected override IReadOnlyList<WaybillSale> GetData(IDbConnection connection, Guid? id = null)
    {
        QueryParameters parameters = new()
        {
            Table = "waybill_sale_view"
        };

        return DefaultQuery(connection, id, parameters)
            .Get<WaybillSale, State, Contractor, Contract>(
                (pr, state, contractor, contract) =>
                {
                    pr.State = state;
                    pr.Contractor = contractor;
                    pr.Contract = contract;
                    return pr;
                })
            .ToList();
    }

    protected override void OnCopyNestedRows(IDbConnection connection, WaybillSale from, WaybillSale to, IDbTransaction? transaction = null)
    {
        base.OnCopyNestedRows(connection, from, to, transaction);

        waybillRepository.CopyContent(connection, from, to, transaction);
    }

    protected override Query FilterQuery(Query query)
    {
        return base
            .FilterQuery(query)
            .OrWhereColumns("paid", "!=", "full_cost");
    }
}
