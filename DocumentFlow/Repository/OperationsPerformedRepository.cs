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

public class OperationsPerformedRepository : DocumentRepository<OperationsPerformed>, IOperationsPerformedRepository, ITransientLifetime 
{
    public OperationsPerformedRepository(IDatabase database) : base(database) { }

    public IReadOnlyList<OperationsPerformed> GetOperations(ProductionLot lot)
    {
        using var conn = GetConnection();
        return GetOperations(conn, lot);
    }

    public IReadOnlyList<OperationsPerformed> GetOperations(IDbConnection connection, ProductionLot lot)
    {
        return connection.GetQuery<OperationsPerformed>()
            .WhereFalse("t0.deleted")
            .WhereTrue("t0.carried_out")
            .Where("t0.owner_id", lot.Id)
            .MappingQuery<OperationsPerformed>(x => x.Operation)
            .MappingQuery<OperationsPerformed>(x => x.Employee)
            .Get<OperationsPerformed, CalculationOperation, OurEmployee>((performed, operation, emp) =>
            {
                performed.Operation = operation;
                performed.Employee = emp;
                return performed;
            })
            .ToList();
    }

    public IReadOnlyList<Employee> GetWorkedEmployes(ProductionLot lot)
    {
        using var conn = GetConnection();
        return GetWorkedEmployes(conn, lot);
    }

    public IReadOnlyList<Employee> GetWorkedEmployes(IDbConnection connection, ProductionLot lot)
    {
        QueryParameters parameters = new()
        {
            Quantity = QuantityInformation.None
        };

        return connection.GetQuery<OperationsPerformed>(parameters)
            .Distinct()
            .Select("e.{id, item_name}")
            .Join("our_employee as e", "e.id", "t0.employee_id")
            .Where("t0.owner_id", lot.Id)
            .WhereFalse("t0.deleted")
            .WhereTrue("t0.carried_out")
            .Get<OurEmployee>()
            .ToList();
    }
}