//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models;

public class MenuItemModel
{
    public string? Header { get; set; }
    public bool IsChecked { get; set; }
    public bool IsEnabled { get; set; }
    public object? Tag { get; set; }
    public object? PlacementTarget { get; set; }

    public override string ToString() => Header ?? string.Empty;
}
