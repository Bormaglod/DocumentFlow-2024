﻿//-----------------------------------------------------------------------
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

public class PurchaseRequestRepository : DocumentRepository<PurchaseRequest>, IPurchaseRequestRepository, ITransientLifetime
{
    public PurchaseRequestRepository(IDatabase database) : base(database) { }

    public void CopyContent(PurchaseRequest requestFrom, PurchaseRequest requestTo)
    {
        using var conn = GetConnection();
        using var transaction = conn.BeginTransaction();

        try
        {
            CopyContent(conn, requestFrom, requestTo, transaction);
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new RepositoryException(ExceptionHelper.Message(e));
        }
    }

    public void CopyContent(IDbConnection connection, PurchaseRequest requestFrom, PurchaseRequest requestTo, IDbTransaction? transaction)
    {
        var sql = "insert into purchase_request_price (owner_id, reference_id, amount, price, product_cost, tax, tax_value, full_cost) select :id_to, prp.reference_id, prp.amount, prp.price, prp.product_cost, prp.tax, prp.tax_value, prp.full_cost from purchase_request_price prp where owner_id = :id_from";
        connection.Execute(sql, new { id_to = requestTo.Id, id_from = requestFrom.Id }, transaction: transaction);
    }

    public IList<PurchaseRequestPrice> GetContent(PurchaseRequest request)
    {
        using var conn = GetConnection();
        return GetContent(conn, request);
    }

    public IList<PurchaseRequestPrice> GetContent(IDbConnection connection, PurchaseRequest request)
    {
        return connection.GetQuery<PurchaseRequestPrice>()
            .Where("t0.owner_id", request.Id)
            .MappingQuery<PurchaseRequestPrice>(x => x.Product)
            .MappingQuery<Product>(x => x.Measurement)
            .Get<PurchaseRequestPrice, Material, Measurement>((prp, material, measurement) =>
            {
                material.Measurement = measurement;
                prp.Product = material;
                return prp;
            })
            .ToList();
    }
}