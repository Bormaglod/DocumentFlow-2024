//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

namespace DocumentFlow.Models;

public class EmailInfo
{
    public required EmailAddress From { get; init; }
    public required IEnumerable<EmailAddress> To { get; init; }
    public required IEnumerable<string> Attachments { get; init; }
    public string? Subject { get; init; }
    public string? Body { get; init; }
}
