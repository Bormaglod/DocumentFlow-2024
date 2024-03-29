//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Messaging.Messages;

using DocumentFlow.Interfaces;

namespace DocumentFlow.Messages;

public class RequestClosePageMessage : ValueChangedMessage<IPageView>
{
    public RequestClosePageMessage(IPageView value) : base(value) { }
}
