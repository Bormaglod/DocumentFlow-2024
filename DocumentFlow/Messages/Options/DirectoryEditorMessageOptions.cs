//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

namespace DocumentFlow.Messages.Options;

public class DirectoryEditorMessageOptions : DocumentEditorMessageOptions
{
    public DirectoryEditorMessageOptions(DocumentInfo? document, DocumentInfo? parent) : base(document)
    {
        Parent = parent;
    }

    public DocumentInfo? Parent { get; }
}
