//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DocumentFlow.Messages;

public class PageClosedMessage : ValueChangedMessage<object>
{
    public PageClosedMessage(object value) : base(value) { }
}