//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public class OrganizationRepository : CompanyRepository<Organization>, IOrganizationRepository, ITransientLifetime
{
    public OrganizationRepository(IDatabase database) : base(database) { }

    public IReadOnlyList<EmailAddress> GetEmails()
    {
        using var conn = GetConnection();
        return GetEmails(conn);
    }

    public IReadOnlyList<EmailAddress> GetEmails(IDbConnection connection)
    {
        return GetSlimQuery(connection)
            .Select("email")
            .WhereNotNull("item_name")
            .WhereNotNull("email")
            .Get<Organization>()
            .Select(x => new EmailAddress(x))
            .ToList();
    }
}
