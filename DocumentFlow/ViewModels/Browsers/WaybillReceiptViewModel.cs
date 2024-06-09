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

public class WaybillReceiptViewModel : WaybillViewModel<WaybillReceipt>, ISelfTransientLifetime
{
    private readonly IWaybillReceiptRepository requestRepository = null!;

    public WaybillReceiptViewModel() { }

    public WaybillReceiptViewModel(IDatabase database, IWaybillReceiptRepository requestRepository, IConfiguration configuration, ILogger<WaybillReceiptViewModel> logger) 
        : base(database, configuration, logger) 
    {
        this.requestRepository = requestRepository;
    }

    public override Type? GetEditorViewType() => typeof(Views.Editors.WaybillReceiptView);
    
    protected override IReadOnlyList<WaybillReceipt> GetData(IDbConnection connection, Guid? id = null)
    {
        QueryParameters parameters = new()
        {
            Table = "waybill_receipt_view"
        };

        return DefaultQuery(connection, id, parameters)
            .Get<WaybillReceipt, State, Contractor, Contract>(
                (pr, state, contractor, contract) =>
                {
                    pr.State = state;
                    pr.Contractor = contractor;
                    pr.Contract = contract;
                    return pr;
                })
            .ToList();
    }

    protected override void OnCopyNestedRows(IDbConnection connection, WaybillReceipt from, WaybillReceipt to, IDbTransaction? transaction = null)
    {
        base.OnCopyNestedRows(connection, from, to, transaction);

        requestRepository.CopyContent(connection, from, to, transaction);
    }

    protected override Query FilterQuery(Query query)
    {
        return base
            .FilterQuery(query)
            .OrWhereColumns("t0.paid", "!=", "t0.full_cost");
    }
}
