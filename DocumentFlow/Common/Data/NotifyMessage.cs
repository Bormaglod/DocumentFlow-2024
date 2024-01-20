//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;

using System.Text.Json.Serialization;

namespace DocumentFlow.Common.Data;

public class NotifyMessage
{
    [JsonPropertyName("destination")]
    public MessageDestination Destination { get; set; }

    [JsonPropertyName("entity-name")]
    public string? EntityName { get; set; }

    [JsonPropertyName("object-id")]
    public Guid ObjectId { get; set; }

    [JsonPropertyName("action")]
    public MessageAction Action { get; set; }
}
