//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Exceptions;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;
using SqlKata.Execution;

using System.Data;


namespace DocumentFlow.Repository;

public class ProductionOrderRepository : DocumentRepository<ProductionOrder>, IProductionOrderRepository, ITransientLifetime
{
    public ProductionOrderRepository(IDatabase database) : base(database) { }

    public IReadOnlyList<ProductionOrder> GetActive(ProductionOrder? includingOrder)
    {
        using var conn = GetConnection();
        return GetActive(conn, includingOrder);
    }

    public IReadOnlyList<ProductionOrder> GetActive(IDbConnection connection, ProductionOrder? includingOrder)
    {
        var q = new Query("production_order_price")
            .Select("owner_id")
            .SelectRaw("sum(product_cost) as product_cost")
            .SelectRaw("sum(tax_value) as tax_value")
            .SelectRaw("sum(full_cost) as full_cost")
            .GroupBy("owner_id");

        return connection.GetQuery<ProductionOrder>()
            .Select("q.*")
            .Where(q => q
                .WhereTrue("t0.carried_out")
                .WhereFalse("t0.deleted")
                .WhereNotIn("t0.state_id", new short[] { State.Unknown, State.Compiled, State.Canceled, State.Completed }))
            .MappingQuery<ProductionOrder>(x => x.Contractor)
            .MappingQuery<ProductionOrder>(x => x.State)
            .LeftJoin(q.As("q"), j => j.On("q.owner_id", "t0.id"))
            .When(includingOrder != null, q => q
                .OrWhere("t0.id", includingOrder!.Id)
            )
            .Get<ProductionOrder, Contractor, State>((pr, contractor, state) =>
            {
                pr.Contractor = contractor;
                pr.State = state;
                return pr;
            })
            .OrderBy(x => x.DocumentDate)
            .ThenBy(x => x.DocumentNumber)
            .ToList();
    }

    public void CopyContent(ProductionOrder orderFrom, ProductionOrder orderTo)
    {
        using var conn = GetConnection();
        using var transaction = conn.BeginTransaction();

        try
        {
            CopyContent(conn, orderFrom, orderTo, transaction);
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new RepositoryException(ExceptionHelper.Message(e));
        }
    }

    public void CopyContent(IDbConnection connection, ProductionOrder orderFrom, ProductionOrder orderTo, IDbTransaction? transaction)
    {
        var sql = "insert into production_order_price (owner_id, reference_id, amount, price, product_cost, tax, tax_value, full_cost, calculation_id) select :id_to, prp.reference_id, prp.amount, prp.price, prp.product_cost, prp.tax, prp.tax_value, prp.full_cost, prp.calculation_id from production_order_price prp where owner_id = :id_from";
        connection.Execute(sql, new { id_to = orderTo.Id, id_from = orderFrom.Id }, transaction: transaction);
    }

    public IList<ProductionOrderPrice> GetContent(ProductionOrder order)
    {
        using var conn = GetConnection();
        return GetContent(conn, order);
    }

    public IList<ProductionOrderPrice> GetContent(IDbConnection connection, ProductionOrder order)
    {
        return connection.GetQuery<ProductionOrderPrice>()
            .Where("t0.owner_id", order.Id)
            .MappingQuery<ProductionOrderPrice>(x => x.Product)
            .MappingQuery<Product>(x => x.Measurement)
            .MappingQuery<ProductionOrderPrice>(x => x.Calculation)
            .Get<ProductionOrderPrice, Goods, Measurement, Calculation>((prp, goods, measurement, calculation) =>
            {
                goods.Measurement = measurement;
                prp.Product = goods;
                prp.Calculation = calculation;
                return prp;
            })
            .ToList();
    }

    public IReadOnlyList<Goods> GetProducts(ProductionOrder order)
    {
        using var conn = GetConnection();
        return GetProducts(conn, order);
    }

    public IReadOnlyList<Goods> GetProducts(IDbConnection connection, ProductionOrder order)
    {
        QueryParameters parameters = new()
        {
            Quantity = QuantityInformation.None
        };

        return connection.GetQuery<ProductionOrder>(parameters)
            .Distinct()
            .Select("g.*")
            .Join("production_order_price as pop", "t0.id", "pop.owner_id")
            .Join("goods as g", "g.id", "pop.reference_id")
            .Where("t0.id", order.Id)
            .Get<Goods>()
            .ToList();
    }
}
