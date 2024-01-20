//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

namespace DocumentFlow.Common;

public class OpenDocumentContext
{
    public OpenDocumentContext(DocumentInfo owner, DocumentRefs document) 
    { 
        Owner = owner;
        Document = document;
    }

    public DocumentInfo Owner { get; }
    public DocumentRefs Document { get; }
}
