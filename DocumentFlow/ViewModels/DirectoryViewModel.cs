﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Common.Data;
using DocumentFlow.Common.Messages;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;

using SqlKata;

using Syncfusion.Windows.Shared;

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

namespace DocumentFlow.ViewModels;

public abstract partial class DirectoryViewModel<T> : EntityGridViewModel<T>
    where T : Directory
{
    private T? parent;

    [ObservableProperty]
    private ObservableCollection<HierarchyItemModel> hierarchyItemsSource = new();

    [ObservableProperty]
    private object? selectedFolder;

    public DirectoryViewModel()
    {
        InitializeHierarchy();
        InitializeToolBar();
    }

    public DirectoryViewModel(IDatabase database) : base(database)
    {
        InitializeHierarchy();
        InitializeToolBar();
    }

    public T? Parent => parent;

    #region Commands

    #region CreateGroup

    private ICommand? createGroup;

    public ICommand CreateGroup
    {
        get
        {
            createGroup ??= new DelegateCommand(OnСreateGroup);
            return createGroup;
        }
    }

    private void OnСreateGroup(object parameter)
    {
        var window = new FolderWindow((code, name) =>
        {
            if (CreateDirectoryGroup(code, name, out T? folder))
            {
                if (DataSource is IList<T> list)
                {
                    list.Add(folder);
                }

                CurrentFolder.HierarchyItems.Add(new HierarchyItemModel(folder));

                return true;
            }

            return false;
        });

        window.ShowDialog();
    }

    #endregion

    #region EditCurrentRow

    protected override void OnEditCurrentRow(object parameter)
    {
        if (SelectedItem is Directory directory && directory.IsFolder)
        {
            var window = new FolderWindow(directory, (code, name) =>
            {
                if (UpdateDirectoryGroup(directory, code, name))
                {
                    directory.Code = code;
                    directory.ItemName = name;

                    var folder = CurrentFolder.HierarchyItems.FirstOrDefault(x => x.Folder?.Id == directory.Id);
                    if (folder != null)
                    {
                        folder.ContentString = name;
                    }

                    return true;
                }

                return false;
            });

            window.ShowDialog();
        }
        else
        {
            base.OnEditCurrentRow(parameter);
        }
    }

    #endregion

    #region SelectCurrentRow

    protected override void OnSelectCurrentRow(object parameter)
    {
        if (SelectedItem is Directory directory && directory.IsFolder)
        {
            RefreshDataSource(directory);
        }
        else
        {
            base.OnSelectCurrentRow(parameter);
        }
    }

    #endregion

    #endregion

    protected override MessageOptions GetEditorOptions() => new DirectoryEditorMessageOptions(Owner, Parent);

    private HierarchyItemModel CurrentFolder
    {
        get
        {
            if (SelectedFolder == null)
            {
                return HierarchyItemsSource[0];
            }
            else
            {
                return (HierarchyItemModel)SelectedFolder;
            }
        }
    }

    public void RefreshDataSource(Directory directory)
    {
        var folder = CurrentFolder.HierarchyItems.FirstOrDefault(x => x.Folder?.Id == directory.Id);
        if (folder != null)
        {
            SelectedFolder = folder;
        }
    }

    protected override bool CheckDeleteRow(T row)
    {
        if (row.IsFolder)
        {
            return MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        return base.CheckDeleteRow(row);
    }

    protected override bool CheckWipeRow(T row)
    {
        if (row.IsFolder)
        {
            return MessageBox.Show("Удаление группы приведет к аналогичному действию для всего содержимого группы. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        return base.CheckWipeRow(row);
    }

    protected override bool CheckCopyRow(T row)
    {
        if (row.IsFolder)
        {
            MessageBox.Show("Копию группы создавать нельзя. Создайте её вручную.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return base.CheckCopyRow(row);
    }

    protected override Query SelectQuery(Query query)
    {
        return base.SelectQuery(query).When(
            Parent == null,
            q => q.WhereNull("t0.parent_id"),
            q => q.Where("t0.parent_id", Parent!.Id));
    }

    protected override void OnAfterRefreshDataSource()
    {
        InitializeHierarchy();

        try
        {
            using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

            var folders = conn.Query<T>($"select * from {EntityProperties.GetTableName(typeof(T))} where is_folder and not deleted order by item_name");

            PopulateHierarchyNode(folders, HierarchyItemsSource[0]);
        }
        catch (Exception e)
        {
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static void PopulateHierarchyNode(IEnumerable<T> folders, HierarchyItemModel hierarchyItem, Guid? parent = null)
    {
        foreach (var folder in folders.Where(x => x.ParentId == parent).OrderBy(x => x.ItemName))
        {
            var newNode = new HierarchyItemModel(folder);
            hierarchyItem.HierarchyItems.Add(newNode);

            PopulateHierarchyNode(folders, newNode, folder.Id);
        }
    }

    private void InitializeHierarchy()
    {
        HierarchyItemsSource.Clear();
        HierarchyItemsSource.Add(new HierarchyItemModel("Домой"));
    }

    private void InitializeToolBar()
    {
        ToolBarItems.AddButtons(this,
            new ToolBarButtonModel("Создать", "file-add") { Command = CreateRow },
            new ToolBarButtonModel("Изменить", "file-edit") { Command = EditCurrentRow },
            new ToolBarButtonModel("Пометить", "file-delete") { Command = SwapMarkedRow },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Удалить", "trash") { Command = WipeRows },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Копия", "copy-edit"),
            new ToolBarGroupingButtonModel("Группа", "folder-add") { Command = CreateGroup },
            new ToolBarSeparatorModel(),
            new ToolBarButtonComboModel("Печать", "print"),
            new ToolBarButtonModel("Настройки", "settings"));
    }

    private bool CreateDirectoryGroup(string code, string name, [MaybeNullWhen(false)] out T createdFolder)
    {
        try
        {
            using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
            using var trans = conn.BeginTransaction();
            try
            {
                string sql = $"insert into {EntityProperties.GetTableName(typeof(T) )} (is_folder, code, item_name, parent_id) values (true, :code, :item_name, :parent) returning id";
                var res = conn.QuerySingle<Guid>(sql, new { code, item_name = name, parent = Parent?.Id }, trans);

                createdFolder = conn.QuerySingle<T>($"select * from {EntityProperties.GetTableName(typeof(T))} where id = :Id", new { Id = res });

                trans.Commit();

                return true;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            createdFolder = null;
            return false;
        }
    }

    private static bool UpdateDirectoryGroup(Directory directory, string code, string name)
    {
        try
        {
            using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
            using var trans = conn.BeginTransaction();
            try
            {
                string sql = $"update {EntityProperties.GetTableName(typeof(T))} set code = :code, item_name = :name where id = :id";
                conn.Execute(sql, new { code, name, id = directory.Id }, trans);

                trans.Commit();

                return true;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    partial void OnSelectedFolderChanged(object? value)
    {
        if (value is HierarchyItemModel hierarchy)
        {
            if (hierarchy.Folder is T folder)
            {
                parent = folder;
            }
            else
            {
                parent = default;
            }

            RefreshDataSource();
        }
    }
}