//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

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
}