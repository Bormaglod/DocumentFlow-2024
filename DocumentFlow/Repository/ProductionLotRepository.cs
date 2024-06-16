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

using SqlKata;

using System.Data;

namespace DocumentFlow.Repository;

public class ProductionLotRepository(IDatabase database) : 
    DocumentRepository<ProductionLot>(database), 
    IProductionLotRepository, 
    ITransientLifetime
{
    public IReadOnlyList<ProductionLot> GetInProgress(ProductionLot? lot)
    {
        using var conn = GetConnection();
        return GetInProgress(conn, lot);
    }

    public IReadOnlyList<ProductionLot> GetInProgress(IDbConnection connection, ProductionLot? lot)
    {
        return GetList(GetInProgressQuery(connection, lot).OrWhere("t0.state_id", State.InActive));
    }

    public IReadOnlyList<ProductionLot> GetActive(ProductionLot? lot)
    {
        using var conn = GetConnection();
        return GetActive(conn, lot);
    }

    public IReadOnlyList<ProductionLot> GetActive(IDbConnection connection, ProductionLot? lot)
    {
        return GetList(GetActiveQuery(connection, lot));
    }

    public IReadOnlyList<ProductionLot> GetActive(Contractor contractor, Goods product, ProductionLot? lot)
    {
        using var conn = GetConnection();
        return GetActive(conn, contractor, product, lot);
    }

    public IReadOnlyList<ProductionLot> GetActive(IDbConnection connection, Contractor contractor, Goods product, ProductionLot? lot)
    {
        return GetList(
            GetActiveQuery(connection, lot, q => q.Where("po.contractor_id", contractor.Id).Where("g.id", product.Id))
                .Join("production_order as po", "t0.owner_id", "po.id"));
    }

    public IReadOnlyList<ProductionLot> GetActive(Goods product, ProductionLot? lot)
    {
        using var conn = GetConnection();
        return GetActive(conn, product, lot);
    }

    public IReadOnlyList<ProductionLot> GetActive(IDbConnection connection, Goods product, ProductionLot? lot)
    {
        return GetList(GetActiveQuery(connection, lot, q => q.Where("g.id", product.Id)));
    }

    public IReadOnlyList<ProductionLot> GetActive(Contractor contractor, ProductionLot? lot)
    {
        using var conn = GetConnection();
        return GetActive(conn, contractor, lot);
    }

    public IReadOnlyList<ProductionLot> GetActive(IDbConnection connection, Contractor contractor, ProductionLot? lot)
    {
        return GetList(
            GetActiveQuery(connection, lot, q => q.Where("po.contractor_id", contractor.Id))
                .Join("production_order as po", "t0.owner_id", "po.id"));
    }

    private static List<ProductionLot> GetList(Query query)
    {
        return query
            .Get<ProductionLot, ProductionOrder, Calculation, Goods>((lot, order, calc, goods) =>
            {
                calc.Goods = goods;

                lot.Order = order;
                lot.Calculation = calc;

                return lot;
            })
            .ToList();
    }

    private static Query GetInProgressQuery(IDbConnection connection, ProductionLot? lot)
    {
        return connection.GetQuery<ProductionLot>()
            .MappingQuery<ProductionLot>(x => x.Order)
            .MappingQuery<ProductionLot>(x => x.Calculation, joinType: JoinType.Inner)
            .MappingQuery<Calculation>(x => x.Goods, joinType: JoinType.Inner, alias: "g")
            .When(lot != null, q => q.OrWhere("t0.id", lot!.Id));
    }

    private static Query GetActiveQuery(IDbConnection connection, ProductionLot? lot, Func<Query, Query>? whereAction = null)
    {
        var fg = new Query("finished_goods")
            .Select("goods_id")
            .Select("owner_id as lot_id")
            .SelectRaw("sum(quantity) as finished_quantity")
            .WhereTrue("carried_out")
            .GroupBy("owner_id", "goods_id");

        var sales = new Query("waybill_sale_price_goods as wspg")
            .Select("wspg.lot_id", "wspg.reference_id")
            .SelectRaw("sum(wspg.amount) as sale_quantity")
            .Join("waybill_sale as ws", "wspg.owner_id", "ws.id")
            .WhereTrue("ws.carried_out")
            .GroupBy("wspg.lot_id", "wspg.reference_id");

        var where = new Query()
            .WhereIn("t0.state_id", [State.InActive, State.Completed])
            .WhereRaw("iif(g.is_service, t0.quantity, coalesce(fg.finished_quantity, 0)) != coalesce(s.sale_quantity, 0)");
        whereAction?.Invoke(where);

        return GetInProgressQuery(connection, lot)
            //connection.GetQuery<ProductionLot>()
            .SelectRaw("iif(g.is_service, t0.quantity, coalesce(fg.finished_quantity, 0)) - coalesce(s.sale_quantity, 0) as free_quantity")
            //.MappingQuery<ProductionLot>(x => x.Order)
            //.MappingQuery<ProductionLot>(x => x.Calculation, joinType: JoinType.Inner)
            //.MappingQuery<Calculation>(x => x.Goods, joinType: JoinType.Inner, alias: "g")
            .LeftJoin(fg.As("fg"), q => q.On("fg.lot_id", "t0.id"))
            .LeftJoin(sales.As("s"), s => s.On("s.lot_id", "t0.id").On("s.reference_id", "g.id"))
            .Where(q => q
                .Where(GetWhereQuery)
                .When(whereAction != null, whereAction)
            );
            //.When(lot != null, q => q.OrWhere("t0.id", lot!.Id));
    }

    private static Query GetWhereQuery(Query query)
    {
        var where = query
            .WhereIn("t0.state_id", [State.InActive, State.Completed])
            .WhereRaw("iif(g.is_service, t0.quantity, coalesce(fg.finished_quantity, 0)) != coalesce(s.sale_quantity, 0)");
        
        return where;
    }
}
