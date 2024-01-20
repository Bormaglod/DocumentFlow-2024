﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace DocumentFlow.Models;

public class ToolBarButtonComboModel : ToolBarButtonModel
{
    private readonly ObservableCollection<MenuItemModel> items = new();

    public ToolBarButtonComboModel(string label) : base(label)
    {
        Items = new(items);
    }

    public ToolBarButtonComboModel(string label, string iconName) : base(label, iconName)
    {
        Items = new(items);
    }

    public ReadOnlyObservableCollection<MenuItemModel>? Items { get; }
}