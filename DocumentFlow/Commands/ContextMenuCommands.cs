//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using Syncfusion.Windows.Shared;

using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace DocumentFlow.Commands;

public static class ContextMenuCommands
{
    #region ClearSummary

    static ICommand? clearSummary;

    public static ICommand ClearSummary
    {
        get
        {
            clearSummary ??= new BaseCommand(OnClearSummary);
            return clearSummary;
        }
    }

    private static void OnClearSummary(object parameter)
    {
        if (parameter is GridRecordContextMenuInfo info)
        {
            var grid = info.DataGrid;
            if (grid.GroupSummaryRows.Any())
            {
                grid.GroupSummaryRows.Clear();
            }
        }
    }

    #endregion

    #region ClearGroups

    static ICommand? clearGroups;

    public static ICommand ClearGroups
    {
        get
        {
            clearGroups ??= new BaseCommand(OnClearGroups, CanClearGroups);
            return clearGroups;
        }
    }

    private static void OnClearGroups(object parameter)
    {
        if (parameter is GridContextMenuInfo info)
        {
            info.DataGrid.GroupColumnDescriptions.Clear();
        }
    }

    private static bool CanClearGroups(object parameter)
    {
        if (parameter is GridContextMenuInfo info)
        {
            var grid = info.DataGrid;
            return grid.GroupColumnDescriptions != null && grid.GroupColumnDescriptions.Any();
        }

        return false;
    }

    #endregion

    #region ClearGroup

    static ICommand? clearGroup;

    public static ICommand ClearGroup
    {
        get
        {
            clearGroup ??= new BaseCommand(OnClearGroup);
            return clearGroup;
        }
    }

    private static void OnClearGroup(object parameter)
    {
        if (parameter is GridColumnContextMenuInfo info)
        {
            var grid = info.DataGrid;
            var column = info.Column;
            if (grid.GroupColumnDescriptions.Any(x => x.ColumnName == column.MappingName))
            {
                grid.GroupColumnDescriptions.Remove(grid.GroupColumnDescriptions.FirstOrDefault(x => x.ColumnName == column.MappingName));
            }
        }
    }

    #endregion

    #region ExpandAll

    static ICommand? expandAll;

    public static ICommand ExpandAll
    {
        get
        {
            expandAll ??= new BaseCommand(OnFullExpand, CanFullExpand);
            return expandAll;
        }
    }

    private static void OnFullExpand(object parameter)
    {
        if (parameter is GridContextMenuInfo info)
        {
            info.DataGrid.ExpandAllGroup();
        }
    }

    private static bool CanFullExpand(object parameter)
    {
        if (parameter is GridContextMenuInfo info)
        {
            var grid = info.DataGrid;
            if (grid.View != null && grid.View.TopLevelGroup != null && grid.View.TopLevelGroup.Groups.Count > 0)
            {
                return grid.View.TopLevelGroup.Groups.Any(x => !x.IsExpanded);
            }
        }

        return false;
    }

    #endregion

    #region CollapseAll

    static ICommand? collapseAll;
    public static ICommand CollapseAll
    {
        get
        {
            collapseAll ??= new BaseCommand(OnFullCollapse, CanFullCollapse);
            return collapseAll;
        }
    }

    private static void OnFullCollapse(object parameter)
    {
        if (parameter is GridContextMenuInfo info)
        {
            info.DataGrid.CollapseAllGroup();
        }
    }

    private static bool CanFullCollapse(object parameter)
    {
        if (parameter is GridContextMenuInfo info)
        {
            var grid = info.DataGrid;
            if (grid.View != null && grid.View.TopLevelGroup != null && grid.View.TopLevelGroup.Groups.Count > 0)
            {
                return grid.View.TopLevelGroup.Groups.Any(x => x.IsExpanded);
            }
        }

        return false;
    }

    #endregion

    #region SortAscending

    private static ICommand? sortAscending;

    public static ICommand SortAscending
    {
        get
        {
            sortAscending ??= new BaseCommand(OnSortAscending, CanSortAscending);
            return sortAscending;
        }
    }

    private static void OnSortAscending(object parameter)
    {
        if (parameter is GridColumnContextMenuInfo info)
        {
            var grid = info.DataGrid;
            var column = info.Column;
            if (grid.SortColumnDescriptions == null)
            {
                return;
            }

            grid.SortColumnDescriptions.Clear();
            grid.SortColumnDescriptions.Add(new SortColumnDescription() { ColumnName = column.MappingName, SortDirection = ListSortDirection.Ascending });
        }
    }

    private static bool CanSortAscending(object parameter)
    {
        if (parameter is GridColumnContextMenuInfo info)
        {
            var grid = info.DataGrid;
            var column = info.Column;
            if (grid.SortColumnDescriptions != null)
            {
                var sortColumn = grid.SortColumnDescriptions.FirstOrDefault(x => x.ColumnName == column.MappingName);
                if (sortColumn != null)
                {
                    if (sortColumn.SortDirection == ListSortDirection.Ascending)
                    {
                        return false;
                    }
                }
            }

            return grid.AllowSorting;
        }

        return false;
    }

    #endregion

    #region SortDescending

    private static ICommand? sortDescending;

    public static ICommand SortDescending
    {
        get
        {
            sortDescending ??= new BaseCommand(OnSortDescending, CanSortDescending);
            return sortDescending;
        }
    }

    private static void OnSortDescending(object parameter)
    {
        if (parameter is GridColumnContextMenuInfo info)
        {
            var grid = info.DataGrid;
            var column = info.Column;
            if (grid.SortColumnDescriptions == null)
            {
                return;
            }

            grid.SortColumnDescriptions.Clear();
            grid.SortColumnDescriptions.Add(new SortColumnDescription() { ColumnName = column.MappingName, SortDirection = ListSortDirection.Descending });
        }
    }

    private static bool CanSortDescending(object parameter)
    {
        if (parameter is GridColumnContextMenuInfo info)
        {
            var grid = info.DataGrid;
            var column = info.Column;
            if (grid.SortColumnDescriptions != null)
            {
                var sortColumn = grid.SortColumnDescriptions.FirstOrDefault(x => x.ColumnName == column.MappingName);
                if (sortColumn != null)
                {
                    if (sortColumn.SortDirection == ListSortDirection.Descending)
                    {
                        return false;
                    }
                }
            }

            return grid.AllowSorting;
        }

        return false;
    }

    #endregion

    #region ClearSorting

    private static ICommand? clearSorting;

    public static ICommand ClearSorting
    {
        get
        {
            clearSorting ??= new BaseCommand(OnClearSorting, CanClearSort);
            return clearSorting;
        }
    }

    private static void OnClearSorting(object parameter)
    {
        if (parameter is GridColumnContextMenuInfo info)
        {
            var grid = info.DataGrid;
            var column = info.Column;
            if (grid.SortColumnDescriptions != null && grid.SortColumnDescriptions.Any(x => x.ColumnName == column.MappingName))
            {
                grid.SortColumnDescriptions.Remove(grid.SortColumnDescriptions.FirstOrDefault(x => x.ColumnName == column.MappingName));
            }
        }
    }

    private static bool CanClearSort(object parameter)
    {
        if (parameter is GridColumnContextMenuInfo info)
        {
            var grid = info.DataGrid;
            var column = info.Column;

            if (grid.SortColumnDescriptions == null)
            {
                return false;
            }

            return grid.SortColumnDescriptions.Any(x => x.ColumnName == column.MappingName);
        }

        return false;
    }

    #endregion

    #region ClearFiltering

    private static ICommand? clearFiltering;

    public static ICommand ClearFiltering
    {
        get
        {
            clearFiltering ??= new BaseCommand(OnClearFiltering, CanClearFiltering);
            return clearFiltering;
        }
    }


    private static void OnClearFiltering(object parameter)
    {
        if (parameter is GridColumnContextMenuInfo info)
        {
            var column = info.Column;
            if (column.FilterPredicates.Any())
            {
                column.FilterPredicates.Clear();
            }
        }
    }

    private static bool CanClearFiltering(object parameter)
    {
        return parameter is GridColumnContextMenuInfo info && info.Column.FilterPredicates.Any();
    }

    #endregion

    #region GroupThisColumn

    private static ICommand? groupThisColumn;

    public static ICommand GroupThisColumn
    {
        get
        {
            groupThisColumn ??= new BaseCommand(OnGroupThisColumn, CanGroupThisColumn);
            return groupThisColumn;
        }
    }


    private static void OnGroupThisColumn(object parameter)
    {
        if (parameter is GridColumnContextMenuInfo info)
        {
            var grid = info.DataGrid;
            var column = info.Column;

            if (grid.GroupColumnDescriptions != null && !grid.GroupColumnDescriptions.Any(x => x.ColumnName == column.MappingName))
            {
                grid.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = column.MappingName });
            }
        }
    }

    private static bool CanGroupThisColumn(object parameter)
    {
        if (parameter is GridColumnContextMenuInfo info)
        {
            var grid = info.DataGrid;
            var column = info.Column;
            bool canGroup = false;
            if (grid.GroupColumnDescriptions != null && !grid.GroupColumnDescriptions.Any(x => x.ColumnName == column.MappingName))
            {
                var groupcolumn = column.ReadLocalValue(GridColumn.AllowGroupingProperty);
                if (grid.AllowGrouping)
                {
                    canGroup = true;
                }

                if (groupcolumn != DependencyProperty.UnsetValue || canGroup)
                {
                    canGroup = column.AllowGrouping;
                }
            }

            return canGroup;
        }

        return false;
    }
    #endregion

    #region ShowHideGroupArea

    private static ICommand? showHideGroupArea;

    public static ICommand ShowHideGroupArea
    {
        get
        {
            showHideGroupArea ??= new BaseCommand(OnShowHideGroupArea);
            return showHideGroupArea;
        }
    }

    private static void OnShowHideGroupArea(object parameter)
    {
        if (parameter is GridContextMenuInfo info)
        {
            var grid = info.DataGrid;
            grid.IsGroupDropAreaExpanded = !grid.IsGroupDropAreaExpanded;
        }
    }

    #endregion

    #region Copy

    private static ICommand? copy;

    public static ICommand Copy
    {
        get
        {
            copy ??= new BaseCommand(OnCopy);
            return copy;
        }
    }


    private static void OnCopy(object parameter)
    {
        if (parameter is GridRecordContextMenuInfo info)
        {
            var grid = info.DataGrid;

            if (grid.CurrentColumn != null && grid.CurrentItem != null)
            {
                string propName = grid.CurrentColumn.MappingName;

                Type type = grid.CurrentItem.GetType();
                PropertyInfo? prop = type.GetProperty(propName);
                if (prop != null)
                {
                    object? value = prop.GetValue(grid.CurrentItem);
                    if (value != null)
                    {
                        Clipboard.SetText(value.ToString() ?? string.Empty);
                    }
                }
            }
        }
    }

    #endregion

    #region CopyIdentifier

    private static ICommand? copyIdentifier;

    public static ICommand CopyIdentifier
    {
        get
        {
            copyIdentifier ??= new BaseCommand(OnCopyIdentifier);
            return copyIdentifier;
        }
    }


    private static void OnCopyIdentifier(object parameter)
    {
        if (parameter is GridRecordContextMenuInfo info)
        {
            var grid = info.DataGrid;

            if (grid.CurrentItem is Identifier identifier)
            {
                Clipboard.SetText(identifier.ToString() ?? string.Empty);
                ToastOperations.IdentifierValueCopied(identifier.Id);
            }
        }
    }

    #endregion

    #region ChangeColumnVisible

    private static ICommand? changeColumnVisible;

    public static ICommand ChangeColumnVisible
    {
        get
        {
            changeColumnVisible ??= new DelegateCommand<MenuItemModel>(OnChangeColumnVisible);
            return changeColumnVisible;
        }
    }

    private static void OnChangeColumnVisible(MenuItemModel parameter)
    {
        if (parameter.Tag is GridColumn column)
        {
            column.IsHidden = !parameter.IsChecked;
        }
    }

    #endregion
}
