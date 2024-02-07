//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models.Entities;

public class Customer : Contractor
{
    public Guid? ContractId { get; protected set; }
    public Guid? ApplicationId { get; protected set; }
    public string? DocNumber { get; protected set; }
    public string? DocName { get; protected set; }
    public DateTime DateStart { get; protected set; }
    public decimal Price { get; protected set; }
}
