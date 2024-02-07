//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Messages.Options;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.Messages;

public class EntityEditorOpenMessage
{
    public EntityEditorOpenMessage(Type viewType)
    {
        EditorType = viewType;
    }

    public EntityEditorOpenMessage(Type editorType, Guid documentId)
    {
        EditorType = editorType;
        DocumentId = documentId;
    }

    public EntityEditorOpenMessage(Type editorType, DocumentInfo document) : this(editorType, document.Id) { }

    public Type EditorType { get; }

    public Guid? DocumentId { get; }

    public DocumentInfo? BasedDocument { get; init; }

    public MessageOptions? Options { get; init; }
}
