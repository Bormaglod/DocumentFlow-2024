//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models;

public class ToolBarNavigationButtonModel : ToolBarButtonModel
{
    public ToolBarNavigationButtonModel(string label) : base(label) { }

    public ToolBarNavigationButtonModel(string label, string iconName) : base(label, iconName) { }
}
