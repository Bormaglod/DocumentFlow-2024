//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;

namespace DocumentFlow.Messages;

public class EntityActionMessage
{
    public EntityActionMessage(string entityName) => (Destination, EntityName) = (MessageDestination.List, entityName);
    public EntityActionMessage(string entityName, Guid ownerId) => (Destination, EntityName, ObjectId) = (MessageDestination.List, entityName, ownerId);
    public EntityActionMessage(string entityName, Guid objectId, MessageAction action) => (Destination, EntityName, ObjectId, Action) = (MessageDestination.Object, entityName, objectId, action);

    public MessageDestination Destination { get; }
    public string EntityName { get; }
    public Guid ObjectId { get; } = Guid.Empty;
    public MessageAction Action { get; } = MessageAction.Refresh;
}