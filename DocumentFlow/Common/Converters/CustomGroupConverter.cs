//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Syncfusion.Data;

namespace DocumentFlow.Common.Converters;

public abstract class CustomGroupConverter : IColumnAccessProvider
{
    public CustomGroupConverter(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public string? ColumnName { get; set; }

    public IPropertyAccessProvider? PropertyReflector { get; set; }

    public string Text { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is CustomGroupConverter converter &&
               Name == converter.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Text);
    }
}
