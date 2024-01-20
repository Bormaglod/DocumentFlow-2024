//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Messages;

public class EntityListOpenMessage
{
    public EntityListOpenMessage(Type viewType, string text)
    {
        ViewType = viewType;
        Text = text;
    }

    public Type ViewType { get; }

    public string Text {  get; }
}
