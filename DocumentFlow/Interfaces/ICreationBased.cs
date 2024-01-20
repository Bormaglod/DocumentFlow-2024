//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Interfaces;

public interface ICreationBased
{
    //string DocumentName { get; }
    Type DocumentEditorType { get; }
    /*bool CanCreateDocument(Type documentType);
    IDocumentInfo Create<T>(T document) where T : class, IDocumentInfo;*/
}
