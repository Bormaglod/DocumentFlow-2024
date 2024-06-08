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

public class EmployeeRepository(IDatabase database) : 
    DirectoryRepository<Employee>(database), 
    IEmployeeRepository, 
    ITransientLifetime
{
    public IReadOnlyList<EmailAddress> GetEmails()
    {
        using var conn = GetConnection();
        return GetEmails(conn);
    }

    public IReadOnlyList<EmailAddress> GetEmails(IDbConnection connection)
    {
        return GetSlimQuery(connection)
            .Select("t0.email")
            .MappingQuery<Employee>(x => x.Company)
            .WhereFalse("t0.deleted")
            .WhereNotNull("t0.item_name")
            .WhereNotNull("t0.email")
            .Get<Employee, Contractor>((emp, contractor) =>
            {
                emp.Company = contractor;
                return emp;
            })
            .Select(x => new EmailAddress(x))
            .ToList();
    }

    public IReadOnlyList<Employee> GetEmployees(Contractor contractor)
    {
        using var conn = GetConnection();
        return GetEmployees(conn, contractor);
    }

    public IReadOnlyList<Employee> GetEmployees(IDbConnection connection, Contractor contractor) => GetSlim(connection, contractor);
}
