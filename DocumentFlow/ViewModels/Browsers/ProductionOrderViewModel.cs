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

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public class ProductionOrderViewModel : DocumentViewModel<ProductionOrder>, ISelfTransientLifetime
{
    private readonly IProductionOrderRepository orderRepository = null!;

    public ProductionOrderViewModel() { }

    public ProductionOrderViewModel(IDatabase database, IProductionOrderRepository orderRepository, IConfiguration configuration)
        : base(database, configuration)
    {
        this.orderRepository = orderRepository;
    }

    public override Type? GetEditorViewType() => typeof(Views.Editors.ProductionOrderView);

    protected override Query SelectQuery(Query query)
    {
        var q = new Query("production_order_price")
            .Select("owner_id")
            .SelectRaw("sum(product_cost) as product_cost")
            .SelectRaw("sum(tax_value) as tax_value")
            .SelectRaw("sum(full_cost) as full_cost")
            .GroupBy("owner_id");

        return base
            .SelectQuery(query)
            .Select("d.{product_cost, tax_value, full_cost}")
            .LeftJoin(q.As("d"), j => j.On("d.owner_id", "t0.id"))
            .MappingQuery<ProductionOrder>(x => x.State)
            .MappingQuery<ProductionOrder>(x => x.Contractor)
            .MappingQuery<ProductionOrder>(x => x.Contract);
    }

    protected override IReadOnlyList<ProductionOrder> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id)
            .Get<ProductionOrder, State, Contractor, Contract>(
                (pr, state, contractor, contract) =>
                {
                    pr.State = state;
                    pr.Contractor = contractor;
                    pr.Contract = contract;
                    return pr;
                })
            .ToList();
    }

    protected override void OnCopyNestedRows(IDbConnection connection, ProductionOrder from, ProductionOrder to, IDbTransaction? transaction = null)
    {
        base.OnCopyNestedRows(connection, from, to, transaction);

        orderRepository.CopyContent(connection, from, to, transaction);
    }
}
