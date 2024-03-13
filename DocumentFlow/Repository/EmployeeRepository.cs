//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Repository;

public class EmployeeRepository : DirectoryRepository<Employee>, IEmployeeRepository, ITransientLifetime
{
    public EmployeeRepository(IDatabase database) : base(database) { }

    public IReadOnlyList<Employee> GetEmployees(Contractor contractor)
    {
        using var conn = GetConnection();
        return GetSlim(conn, contractor);
    }

    public IReadOnlyList<Employee> GetEmployees(IDbConnection connection, Contractor contractor) => GetSlim(connection, contractor);
}
