﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public class OperationRepository(IDatabase database) : 
    DirectoryRepository<Operation>(database), 
    IOperationRepository, 
    ITransientLifetime
{
    public IReadOnlyList<Operation> GetOperations()
    {
        using var conn = GetConnection();
        return GetOperations(conn);
    }

    public IReadOnlyList<Operation> GetOperations(IDbConnection connection)
    {
        var parameters = new QueryParameters()
        {
            FromOnly = true,
            IncludeDocumentsInfo = false,
            Quantity = QuantityInformation.DirectoryExt
        };

        return connection.GetQuery<Operation>(parameters)
            .WhereFalse("deleted")
            .OrderBy("item_name")
            .Select("parent_id", "is_folder", "salary")
            .Get<Operation>()
            .ToList();
    }

    public IList<OperationGoods> GetGoods(Operation operation)
    {
        using var conn = GetConnection();
        return GetGoods(conn, operation);
    }

    public IList<OperationGoods> GetGoods(IDbConnection connection, Operation operation)
    {
        var parameters = new QueryParameters()
        {
            Alias = "og"
        };

        return connection.GetQuery<OperationGoods>(parameters)
            .MappingQuery<OperationGoods>(x => x.Goods, QuantityInformation.Directory)
            .Where("og.owner_id", operation.Id)
            .Get<OperationGoods, Goods>(
                map: (og, goods) =>
                {
                    og.Goods = goods;
                    return og;
                }
            )
            .ToList();
    }
}
