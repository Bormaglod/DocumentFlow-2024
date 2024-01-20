//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Common;

public class CreatingBasedContext
{
    public CreatingBasedContext(Type editorType, string text)
    {
        EditorType = editorType;
        Text = text;
    }

    public Type EditorType { get; }

    public string Text { get; }
}