//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common.Collections;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public class CalculationOperationRepository : CalculationItemRepository<CalculationOperation>, ICalculationOperationRepository, ITransientLifetime
{
    public CalculationOperationRepository(IDatabase database) : base(database) { }

    public IList<CalculationOperationProperty> GetProperties(CalculationOperation operation)
    {
        using var conn = GetConnection();
        return GetProperties(conn, operation);
    }

    public IList<CalculationOperationProperty> GetProperties(IDbConnection connection, CalculationOperation operation)
    {
        var sql = "select cop.*, p.* from calculation_operation_property as cop join property p on p.id = cop.property_id where cop.operation_id = :Id";
        var list = connection.Query<CalculationOperationProperty, Property, CalculationOperationProperty>(sql, (cop, prop) =>
        {
            cop.Property = prop;
            return cop;
        }, operation);

        return new DependentCollection<CalculationOperationProperty>(operation, list);
    }

    public IReadOnlyList<CalculationOperation> GetPreviousOperations(Calculation calculation, CalculationOperation? operation)
    {
        using var conn = GetConnection();
        return GetPreviousOperations(conn, calculation, operation);
    }

    public IReadOnlyList<CalculationOperation> GetPreviousOperations(IDbConnection connection, Calculation calculation, CalculationOperation? operation)
    {
        return GetSlimQuery(connection, calculation, false)
            .When(operation != null, q => q.Where("id", "!=", operation!.Id))
            .OrderBy("code")
            .Get<CalculationOperation>()
            .ToList();
    }
}
