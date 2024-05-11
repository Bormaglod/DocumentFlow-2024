//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IOperationsPerformedRepository : IDocumentRepository<OperationsPerformed>
{
    IReadOnlyList<OperationsPerformed> GetOperations(ProductionLot lot);
    IReadOnlyList<OperationsPerformed> GetOperations(IDbConnection connection, ProductionLot lot);
    IReadOnlyList<Employee> GetWorkedEmployes(ProductionLot lot);
    IReadOnlyList<Employee> GetWorkedEmployes(IDbConnection connection, ProductionLot lot);
}
