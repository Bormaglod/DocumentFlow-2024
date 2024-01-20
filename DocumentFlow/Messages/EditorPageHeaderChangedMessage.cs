//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;

namespace DocumentFlow.Messages;

public class EditorPageHeaderChangedMessage
{
    public EditorPageHeaderChangedMessage(IPageView page, string header) => (Page, Header) = (page, header);

    public IPageView Page { get; }
    public string Header { get; }
}
