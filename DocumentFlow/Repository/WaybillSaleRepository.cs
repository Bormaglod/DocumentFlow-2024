//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Common.Exceptions;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Repository;

public class WaybillSaleRepository(IDatabase database) :
    DocumentRepository<WaybillSale>(database),
    IWaybillSaleRepository,
    ITransientLifetime
{
    public IList<WaybillSalePrice> GetContent(WaybillSale waybill)
    {
        using var conn = GetConnection();
        return GetContent(conn, waybill);
    }

    public IList<WaybillSalePrice> GetContent(IDbConnection connection, WaybillSale waybill)
    {
        return connection.GetQuery<WaybillSalePrice>()
            .SelectRaw("regexp_replace(t0.tableoid::regclass::varchar, '^.*_', '') as discriminator")
            .Where("t0.owner_id", waybill.Id)
            .MappingQuery<WaybillSalePrice>(x => x.Product)
            .MappingQuery<Product>(x => x.Measurement)
            .MappingQuery<WaybillSalePrice>(x => x.Lot)
            .Get<WaybillSalePrice, Product, Measurement, ProductionLot>((prp, product, measurement, lot) =>
            {
                product.Measurement = measurement;
                prp.Product = product;
                prp.Lot = lot;
                return prp;
            })
            .ToList();
    }

    public void CopyContent(WaybillSale waybillFrom, WaybillSale waybillTo)
    {
        using var conn = GetConnection();
        using var transaction = conn.BeginTransaction();

        try
        {
            CopyContent(conn, waybillFrom, waybillTo, transaction);
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new RepositoryException(ExceptionHelper.Message(e));
        }
    }

    public void CopyContent(IDbConnection connection, WaybillSale waybillFrom, WaybillSale waybillTo, IDbTransaction? transaction)
    {
        string[] tables = [ "goods", "material" ];

        foreach (var table in tables)
        {
            var sql = $"insert into waybill_sale_price_{table} (owner_id, reference_id, amount, price, product_cost, tax, tax_value, full_cost) select :id_to, reference_id, amount, price, product_cost, tax, tax_value, full_cost from waybill_sale_price_{table} where owner_id = :id_from";
            connection.Execute(sql, new { id_to = waybillTo.Id, id_from = waybillFrom.Id }, transaction: transaction);
        }
    }
}
