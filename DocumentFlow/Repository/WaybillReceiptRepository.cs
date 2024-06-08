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

public class WaybillReceiptRepository(IDatabase database) : 
    DocumentRepository<WaybillReceipt>(database), 
    IWaybillReceiptRepository, 
    ITransientLifetime
{
    public void CopyContent(WaybillReceipt waybillFrom, WaybillReceipt waybillTo)
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

    public void CopyContent(IDbConnection connection, WaybillReceipt waybillFrom, WaybillReceipt waybillTo, IDbTransaction? transaction)
    {
        var sql = "insert into waybill_receipt_price (owner_id, reference_id, amount, price, product_cost, tax, tax_value, full_cost) select :id_to, prp.reference_id, prp.amount, prp.price, prp.product_cost, prp.tax, prp.tax_value, prp.full_cost from waybill_receipt_price prp where owner_id = :id_from";
        connection.Execute(sql, new { id_to = waybillTo.Id, id_from = waybillFrom.Id }, transaction: transaction);
    }

    public IList<WaybillReceiptPrice> GetContent(WaybillReceipt waybill)
    {
        using var conn = GetConnection();
        return GetContent(conn, waybill);
    }

    public IList<WaybillReceiptPrice> GetContent(IDbConnection connection, WaybillReceipt waybill)
    {
        return connection.GetQuery<WaybillReceiptPrice>()
            .Where("t0.owner_id", waybill.Id)
            .MappingQuery<WaybillReceiptPrice>(x => x.Product)
            .MappingQuery<Product>(x => x.Measurement)
            .Get<WaybillReceiptPrice, Material, Measurement>((prp, material, measurement) =>
            {
                material.Measurement = measurement;
                prp.Product = material;
                return prp;
            })
            .ToList();
    }
}
