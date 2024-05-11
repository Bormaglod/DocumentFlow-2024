//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Common;

public abstract class GridComboColumn
{
    public GridComboColumn() { }

    public GridComboColumn(string mappingName, string header, double width)
    {
        MappingName = mappingName;
        Header = header;
        Width = width;
    }

    public int Order { get; set; } = 0;
    public string MappingName { get; set; } = string.Empty;
    public string Header { get; set; } = string.Empty;
    public double Width { get; set; } = double.NaN;
}
