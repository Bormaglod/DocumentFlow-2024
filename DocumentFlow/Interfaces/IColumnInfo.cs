//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Syncfusion.UI.Xaml.Grid;

namespace DocumentFlow.Interfaces;

public enum ColumnVisibleState { Default, AlwaysVisible, AlwaysHidden }

public interface IColumnInfo
{
    string MappingName { get; }
    GridLengthUnitType ColumnSizer { get; set; }
    double Width { get; set; }
    bool IsHidden { get; set; }
    ColumnVisibleState State { get; set; }
}
