//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Models.Entities;

using System.Collections.ObjectModel;

namespace DocumentFlow.Models;

public partial class HierarchyItemModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<HierarchyItemModel> hierarchyItems = new();

    [ObservableProperty]
    public string contentString;

    public HierarchyItemModel(string content, params HierarchyItemModel[] myItems)
    {
        ContentString = content;
        foreach (var item in myItems)
        {
            HierarchyItems.Add(item);
        }
    }

    public HierarchyItemModel(Directory folder, params HierarchyItemModel[] myItems) : this(folder.ItemName ?? folder.Code, myItems)
    {
        Folder = folder;
    }

    public bool IsRoot => Folder == null;

    public Directory? Folder { get; }
}
