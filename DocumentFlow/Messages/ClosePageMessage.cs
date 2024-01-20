//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Messaging.Messages;

using DocumentFlow.Interfaces;

namespace DocumentFlow.Messages;

public class ClosePageMessage : ValueChangedMessage<IPageView>
{
    public ClosePageMessage(IPageView value) : base(value) { }
}