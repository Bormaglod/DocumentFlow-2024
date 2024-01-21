//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Messages;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.Interfaces;

public interface IEntityEditorViewModel
{
    IEditorPageView? View { get; set; }
    Guid Id { get; }
    string Header { get; }

    DocumentInfo? DocumentInfo { get; }
    void LoadDocument(DocumentInfo info, MessageOptions? options);
    void CreateDocument(MessageOptions? options);
}