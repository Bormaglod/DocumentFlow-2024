﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IOrganizationRepository : ICompanyRepository<Organization>
{
    IReadOnlyList<EmailAddress> GetEmails();
    IReadOnlyList<EmailAddress> GetEmails(IDbConnection connection);
    IReadOnlyList<Organization> GetOrganizations();
    IReadOnlyList<Organization> GetOrganizations(IDbConnection connection);
}