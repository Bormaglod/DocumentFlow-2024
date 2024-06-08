//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;
using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public abstract class ProductRepository<T>(IDatabase database) : DirectoryRepository<T>(database), IProductRepository<T>
    where T : Product
{
    public decimal GetAveragePrice(T product, DateTime? relevanceDate = null)
    {
        using var conn = GetConnection();
        return GetAveragePrice(conn, product, relevanceDate);
    }

    public decimal GetAveragePrice(IDbConnection connection, T product, DateTime? relevanceDate = null)
    {
        return connection.QuerySingle<decimal>(
            "select average_price(:product_id, :relevance_date)",
            new
            {
                product_id = product.Id,
                relevance_date = relevanceDate ?? DateTime.Now
            });
    }

    public decimal GetRemainder(T product, DateTime? actualDate = null)
    {
        using var conn = GetConnection();
        return GetRemainder(conn, product, actualDate);
    }

    public decimal GetRemainder(IDbConnection connection, T product, DateTime? actualDate = null)
    {
        return connection.QuerySingle<decimal>(
            "select get_product_remainder(:product_id, :actual_date)",
            new
            {
                product_id = product.Id,
                actual_date = actualDate ?? DateTime.Now
            });
    }

    protected override Query GetSlimQuery(IDbConnection connection, DocumentInfo? owner = null, bool isDefaultSorting = true)
    {
        return base.GetSlimQuery(connection, owner, isDefaultSorting)
            .Select("t0.price");
    }

    public IReadOnlyList<T> GetProducts(bool withMeasurements = false)
    {
        using var conn = GetConnection();
        return GetProducts(conn, withMeasurements);
    }

    public IReadOnlyList<T> GetProducts(IDbConnection connection, bool withMeasurements = false)
    {
        QueryParameters parameters = new()
        {
            Quantity = QuantityInformation.DirectoryExt
        };

        var query = connection.GetQuery<T>(parameters)
            .Select("t0.price")
            .When(withMeasurements, q => q
                .MappingQuery<T>(x => x.Measurement)
            );

        if (withMeasurements)
        {
            return query.Get<T, Measurement>((product, measurement) =>
            {
                product.Measurement = measurement;
                return product;
            }).ToList();
        }
        else
        {
            return query.Get<T>().ToList();
        }
    }

}
