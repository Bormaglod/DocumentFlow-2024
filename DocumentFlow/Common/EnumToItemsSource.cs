//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// From: https://habr.com/ru/sandbox/98809/
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Windows.Markup;

namespace DocumentFlow.Common;

public class EnumToItemsSource : MarkupExtension
{
    private readonly Type _type;

    public EnumToItemsSource(Type type)
    {
        _type = type;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _type
            .GetMembers()
            .SelectMany(member => member
                .GetCustomAttributes(typeof(DescriptionAttribute), true)
                .Cast<DescriptionAttribute>())
            .Select(x => x.Description)
            .ToList();
    }
}