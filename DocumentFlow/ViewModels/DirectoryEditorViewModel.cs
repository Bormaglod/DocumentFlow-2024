//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using Dapper;

using DocumentFlow.Common.Messages;
using DocumentFlow.Interfaces;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;

using Humanizer;

namespace DocumentFlow.ViewModels;

public abstract partial class DirectoryEditorViewModel<T> : EntityEditorViewModel<T>
    where T : Directory, new()
{
    [ObservableProperty]
    private Guid? parentId;

    [ObservableProperty]
    private IEnumerable<T> folders;

    public DirectoryEditorViewModel() 
    {
        ToolBarItems.AddButtons(this,
            new ToolBarButtonModel("Обновить", "sync") { Command = Refresh },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Сохранить", "save") { Command = Save },
            new ToolBarButtonModel("Сохранить и закрыть", "save-close") { Command = SaveAndClose });

        folders = Array.Empty<T>();
    }

    public DirectoryEditorViewModel(IDatabase database) : this()
    {
        using var conn = database.OpenConnection();
        folders = conn.Query<T>($"select * from {typeof(T).Name.Underscore()} where is_folder");
    }

    protected override void SetOptions(MessageOptions options)
    {
        base.SetOptions(options);
        if (options is DirectoryEditorMessageOptions directoryOptions)
        {
            ParentId = directoryOptions.Parent?.Id;
        }
    }
}
