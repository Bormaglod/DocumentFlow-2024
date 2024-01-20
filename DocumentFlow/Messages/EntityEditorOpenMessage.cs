//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Messages;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.Messages;

public class EntityEditorOpenMessage
{
    public EntityEditorOpenMessage(Type viewType)
    {
        EditorType = viewType;
    }

    public EntityEditorOpenMessage(Type editorType, DocumentInfo document)
    {
        EditorType = editorType;
        Document = document;
    }

    public Type EditorType { get; }

    public DocumentInfo? Document { get; }

    public DocumentInfo? BasedDocument { get; init; }

    public MessageOptions? Options { get; init; }
}
