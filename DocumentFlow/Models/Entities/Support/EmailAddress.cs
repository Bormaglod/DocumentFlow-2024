//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models.Entities;

public class EmailAddress
{
    public EmailAddress(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee.Company, nameof(employee.Company));
        ArgumentNullException.ThrowIfNull(employee.ItemName, nameof(employee.ItemName));
        ArgumentNullException.ThrowIfNull(employee.Email, nameof(employee.Email));

        Company = employee.Company;
        Name = $"{employee.ItemName} ({employee.Company})";
        Email = employee.Email;
    }

    public EmailAddress(Organization organization)
    {
        ArgumentNullException.ThrowIfNull(organization.ItemName, nameof(organization.ItemName));
        ArgumentNullException.ThrowIfNull(organization.Email, nameof(organization.Email));

        Name = organization.ItemName;
        Email = organization.Email;
    }

    public EmailAddress(OurEmployee employee)
    {
        ArgumentNullException.ThrowIfNull(employee.ItemName, nameof(employee.ItemName));
        ArgumentNullException.ThrowIfNull(employee.Email, nameof(employee.Email));

        Name = employee.ItemName;
        Email = employee.Email;
    }

    public Company? Company { get; }
    public string Name { get; }
    public string Email { get; }
    public override string ToString() => $"{Name} <{Email}>";
}
