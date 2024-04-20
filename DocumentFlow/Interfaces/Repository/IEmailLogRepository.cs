//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

namespace DocumentFlow.Interfaces.Repository;

public interface IEmailLogRepository
{
    //IReadOnlyList<EmailLog> GetEmails(Guid? group_id, Guid? contractorId);
    Task LogAsync(EmailLog log);
    Task LogAsync(IEnumerable<EmailLog> logs);
}