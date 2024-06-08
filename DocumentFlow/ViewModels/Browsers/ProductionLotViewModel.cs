//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public class ProductionLotViewModel : DocumentViewModel<ProductionLot>, ISelfTransientLifetime
{
    public ProductionLotViewModel() { }

    public ProductionLotViewModel(IDatabase database, IConfiguration configuration, ILogger<ProductionLotViewModel> logger)
        : base(database, configuration, logger)
    {
    }

    public override Type? GetEditorViewType() => typeof(Views.Editors.ProductionLotView);

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        base.ConfigureColumn(columnInfo);
        if (columnInfo.MappingName == nameof(ProductionLot.Order))
        {
            columnInfo.State = ColumnVisibleState.AlwaysHidden;
        }
    }

    protected override Query SelectQuery(Query query)
    {
        var goods_time = new Query("calculation_operation as co")
            .Select("co.owner_id as calc_id")
            .SelectRaw("sum(co.repeats * (3600.0 / o.production_rate)) as calc_time").Comment("Время изготовления изделия")
            .Join("operation as o", "o.id", "co.item_id")
            .GroupBy("co.owner_id");

        var process = new Query("operations_performed as op")
            .Select("op.owner_id as lot_id")
            .SelectRaw("round(sum(op.quantity * (3600.0 / o.production_rate)) * 100 / (gt.calc_time * pl.quantity), 0) as execute_percent")
            .Join("production_lot as pl", "pl.id", "op.owner_id")
            .Join("calculation_operation as co", "co.id", "op.operation_id")
            .Join("operation as o", "o.id", "co.item_id")
            .Join("goods_time as gt", "gt.calc_id", "pl.calculation_id")
            .WhereTrue("op.carried_out")
            .WhereFalse("op.deleted")
            .GroupBy("op.owner_id", "gt.calc_time", "pl.quantity");

        return base
            .SelectQuery(query)
            .With("goods_time", goods_time)
            .With("process", process)
            .SelectRaw("case when p.execute_percent > 100 and t0.document_date::date <= '31.12.2022' then 100 else p.execute_percent end as execute_percent")
            .MappingQuery<ProductionLot>(x => x.Order)
            .MappingQuery<ProductionLot>(x => x.State)
            .MappingQuery<ProductionLot>(x => x.Calculation)
            .MappingQuery<Calculation>(x => x.Goods)
            .LeftJoin("process as p", "p.lot_id", "t0.id"); ;
    }

    protected override IReadOnlyList<ProductionLot> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id, new QueryParameters())
            .Get<ProductionLot, ProductionOrder, State, Calculation, Goods>(
                (pr, order, state, calculation, goods) =>
                {
                    pr.Order = order;
                    pr.State = state;
                    pr.Calculation = calculation;
                    pr.Calculation.Goods = goods;
                    return pr;
                })
            .ToList();
    }

    protected override Query FilterQuery(Query query)
    {
        return base
            .FilterQuery(query)
            .OrWhere(q => q.WhereFalse("t0.sold"));
    }
}
