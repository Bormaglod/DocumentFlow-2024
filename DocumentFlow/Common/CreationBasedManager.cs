//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Common;

public static class CreationBasedManager
{
    private static readonly Dictionary<Type, List<CreatingBasedContext>> browsers = new();

    public static IEnumerable<CreatingBasedContext> GetEditors(Type browserType)
    {
        if (browsers.TryGetValue(browserType, out List<CreatingBasedContext>? value))
        {
            return value;
        }

        return Array.Empty<CreatingBasedContext>();
    }

    public static void Register(Type browserType, Type editorType, string text)
    {
        if (browsers.TryGetValue(browserType, out List<CreatingBasedContext>? value)) 
        { 
            value.Add(new CreatingBasedContext(editorType, text));
        }
    }
}
