//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

namespace DocumentFlow.Messages.Options;

public class DocumentEditorMessageOptions : MessageOptions
{
    public DocumentEditorMessageOptions(DocumentInfo? owner)
    {
        Owner = owner;
    }

    public DocumentInfo? Owner { get; }
}
