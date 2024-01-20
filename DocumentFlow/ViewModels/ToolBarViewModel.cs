//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models;

using System.Collections.ObjectModel;

namespace DocumentFlow.ViewModels;

public partial class ToolBarViewModel
{
    public ObservableCollection<ToolBarItemModel> Buttons { get; } = new();

    public void AddButtons(object placementTarget, params ToolBarItemModel[] buttons)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].PlacementTarget = placementTarget;
            Buttons.Add(buttons[i]);
        }
    }
}
