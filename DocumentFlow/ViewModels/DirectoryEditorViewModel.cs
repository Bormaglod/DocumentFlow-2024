//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using Dapper;

using DocumentFlow.Messages.Options;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;

using Humanizer;

using System.Data;

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
            new ToolBarButtonModel("Обновить", "sync") { Command = RefreshCommand },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Сохранить", "save") { Command = SaveCommand },
            new ToolBarButtonModel("Сохранить и закрыть", "save-close") { Command = SaveAndCloseCommand },
            new ToolBarSeparatorModel(),
            new ToolBarButtonComboModel("Печать", "print", Reports));

        folders = Array.Empty<T>();
    }

    protected override void InitializeEntityCollections(IDbConnection connection, T? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        Folders = connection.Query<T>($"select * from {typeof(T).Name.Underscore()} where is_folder");
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
