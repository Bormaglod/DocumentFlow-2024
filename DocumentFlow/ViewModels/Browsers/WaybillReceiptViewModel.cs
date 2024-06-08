//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

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

    protected override Query SelectQuery(Query query)
    {
        var q = new Query("waybill_receipt_price")
            .Select("owner_id")
            .SelectRaw("sum(product_cost) as product_cost")
            .SelectRaw("sum(tax_value) as tax_value")
            .SelectRaw("sum(full_cost) as full_cost")
            .GroupBy("owner_id");

        var ppr = GetQuery("posting_payments_receipt", "document_id");
        var ppp = GetQuery("posting_payments_purchase", "document_id");
        var debt = GetQuery("debt_adjustment", "document_debt_id");
        var credit = GetQuery("debt_adjustment", "document_credit_id");

        return base
            .SelectQuery(query)
            .Select("d.{product_cost, tax_value, full_cost}")
            .SelectRaw("coalesce(ppr.transaction_amount, 0) + coalesce(ppp.transaction_amount, 0) + coalesce(credit.transaction_amount, 0) - coalesce(debt.transaction_amount, 0) as paid")
            .Select("pr.document_number as purchase_request_number")
            .Select("pr.document_date as purchase_request_date")
            .LeftJoin("purchase_request as pr", "pr.id", "t0.owner_id")
            .LeftJoin(q.As("d"), j => j.On("d.owner_id", "t0.id"))
            .LeftJoin(ppr.As("ppr"), j => j.On("ppr.document_id", "t0.id"))
            .LeftJoin(ppp.As("ppp"), j => j.On("ppp.document_id", "pr.id"))
            .LeftJoin(debt.As("debt"), j => j.On("debt.document_debt_id", "t0.id"))
            .LeftJoin(credit.As("credit"), j => j.On("credit.document_credit_id", "t0.id"));
    }

    protected override IReadOnlyList<WaybillReceipt> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id)
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

    private static Query GetQuery(string table, string doc)
    {
        return new Query(table)
            .Select(doc)
            .SelectRaw("sum(transaction_amount) as transaction_amount")
            .WhereTrue("carried_out")
            .GroupBy(doc);
    }

    protected override void OnCopyNestedRows(IDbConnection connection, WaybillReceipt from, WaybillReceipt to, IDbTransaction? transaction = null)
    {
        base.OnCopyNestedRows(connection, from, to, transaction);

        requestRepository.CopyContent(connection, from, to, transaction);
    }
}
