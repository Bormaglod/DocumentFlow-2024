﻿//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Common.Comparers;
using DocumentFlow.Common.Converters;
using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Exceptions;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Messages.Options;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;
using DocumentFlow.Models.Settings;
using DocumentFlow.Settings;

using Humanizer;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlKata;
using SqlKata.Execution;

using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Tools.Controls;

using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace DocumentFlow.ViewModels;

public abstract partial class EntityGridViewModel<T> : 
    ObservableObject, 
    IRecipient<EntityActionMessage>, 
    IRecipient<PageClosedMessage>, 
    IEntityGridViewModel, 
    IReport where T : DocumentInfo
{
    private class ColumnInfo(GridColumn gridColumn) : IColumnInfo
    {
        public string MappingName { get; } = gridColumn.MappingName;
        public GridLengthUnitType ColumnSizer { get; set; } = gridColumn.ColumnSizer;
        public double Width { get; set; } = gridColumn.Width;
        public bool IsHidden { get; set; } = gridColumn.IsHidden;
        public ColumnVisibleState State { get; set; } = ColumnVisibleState.Default;
    }

    private readonly IConfiguration configuration;
    private readonly ILogger<EntityGridViewModel<T>> logger;
    private SfDataGrid? grid;
    private readonly List<GridColumn> alwaysVisibleColumns = [];
    private readonly BrowserSettings settings = new();
    private readonly List<MenuItemModel> reports = [];
    private bool isLoaded = false;

    [ObservableProperty]
    private ObservableCollection<T>? dataSource;

    [ObservableProperty]
    private ObservableCollection<MenuItemModel> visibleColumnsMenuItems = [];

    [ObservableProperty]
    public ObservableCollection<MenuItemModel> documentMenuItems = [];

    [ObservableProperty]
    public ObservableCollection<MenuItemModel> creationBasedMenuItems = [];

    [ObservableProperty]
    private object? selectedItem;

    [ObservableProperty]
    private DocumentInfo? owner;

    [ObservableProperty]
    private bool availableNavigation = true;

    [ObservableProperty]
    private bool availableGrouping = true;

    [ObservableProperty]
    private bool isGroupDropAreaExpanded = true;

    [ObservableProperty]
    private SizeMode sizeMode = SizeMode.Normal;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    public EntityGridViewModel() 
    {
        InitializeViewer();
    }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

    public EntityGridViewModel(IDatabase database, IConfiguration configuration, ILogger<EntityGridViewModel<T>> logger)
    {
        CurrentDatabase = database;

        foreach (var context in CreationBasedManager.GetEditors(GetType()))
        {
            var item = new MenuItemModel()
            {
                Header = context.Text,
                Tag = context,
                PlacementTarget = this
            };

            creationBasedMenuItems.Add(item);
        }

        this.configuration = configuration;
        this.logger = logger;

        WeakReferenceMessenger.Default.RegisterAll(this);

        InitializeViewer();
    }

    public ToolBarViewModel ToolBarItems { get; } = new();

    public bool IsDependent { get; set; }

    public bool SupportAccepting => GetSupportAccepting();

    protected IDatabase CurrentDatabase { get; private set; }

    protected IEnumerable<MenuItemModel> Reports => reports;

    protected bool IsLoaded => isLoaded;

    #region Commands

    [RelayCommand]
    private void PopulateListDocuments()
    {
        DocumentMenuItems.Clear();
        if (SelectedItem is T info)
        {
            try
            {
                using var conn = CurrentDatabase.OpenConnection();
                var docs = conn.Query<DocumentRefs>("select * from document_refs where owner_id = :Id", info).ToList();

                foreach (var doc in docs)
                {
                    var item = new MenuItemModel()
                    {
                        Header = string.IsNullOrEmpty(doc.Note) ? doc.FileName : doc.Note,
                        Tag = new OpenDocumentContext(info, doc),
                        PlacementTarget = this
                    };

                    DocumentMenuItems.Add(item);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void OnRefresh() => RefreshDataSource();

    [RelayCommand(CanExecute = nameof(CanCreateRow))]
    private void CreateRow()
    {
        var type = GetEditorViewType();
        if (type != null)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(type) { Options = GetEditorOptions() });
        }
    }

    private bool CanCreateRow() => GetEditorViewType() != null;

    [RelayCommand(CanExecute = nameof(CanEditCurrentRow))]
    protected virtual void EditCurrentRow()
    {
        var type = GetEditorViewType();
        if (SelectedItem is DocumentInfo document && type != null)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(type, document) { Options = GetEditorOptions() });
        }
    }

    private bool CanEditCurrentRow() => GetEditorViewType() != null;

    [RelayCommand]
    protected virtual void SelectCurrentRow() => EditCurrentRow();

    [RelayCommand(CanExecute = nameof(CanSwapMarkedRow))]
    private void SwapMarkedRow()
    {
        if (SelectedItem is T row)
        {
            SetMarkedValue(!row.Deleted);
        }
    }

    private bool CanSwapMarkedRow() => GetEditorViewType() != null;

    [RelayCommand(CanExecute = nameof(CanWipeRows))]
    private void WipeRows()
    {
        var dialog = new WipeConfirmationWindow();
        if (dialog.ShowDialog() == true)
        {
            switch (dialog.Action)
            {
                case WipeAction.Current:
                    WipeCurrentRow();
                    break;
                case WipeAction.All:
                    WipeMarkedRows();
                    break;
            }
        }
    }

    private bool CanWipeRows() => GetEditorViewType() != null;

    [RelayCommand]
    private void ControlLoaded(RoutedEventArgs e)
    {
        if (isLoaded)
        {
            return;
        }

        logger.LogInformation("The {Name} control has loading", typeof(T).Name);

        if (e.Source is not IGridPageView view)
        {
            return;
        }

        grid = view.GetDataGrid();
        if (grid == null)
        {
            return;
        }

        var section = configuration.GetSection(GetConfigFileName());
        section.Bind(settings);

        LoadFilter(section);

        if (settings.Groups != null)
        {
            grid.GroupColumnDescriptions.Clear();
            foreach (var item in settings.Groups.OrderBy(x => x.Order))
            {
                GroupColumnDescription? groupColumn = null;
                if (item.Extended)
                {
                    if (this is ICustomGroupingView customGroupingView) 
                    {
                        foreach (var column in customGroupingView.GroupingColumns)
                        {
                            if (column.Converter is CustomGroupConverter converter && converter.Name == item.Name)
                            {
                                groupColumn = new() { ColumnName = column.MappingName, Converter = column.Converter };
                                break;
                            }
                        }
                    }
                }
                else
                {
                    groupColumn = new() { ColumnName = item.Name };
                }

                if (groupColumn != null)
                {
                    grid.GroupColumnDescriptions.Add(groupColumn);
                }
            }
        }

        foreach (var column in grid.Columns.Where(c => !string.IsNullOrEmpty(c.HeaderText)))
        {
            grid.SortComparers.Add(
                new SortComparer()
                {
                    Comparer = new CustomComparer(typeof(T), column.MappingName),
                    PropertyName = column.MappingName
                });

            ColumnInfo info = new(column);
            ConfigureColumn(info);

            column.Width = info.Width;
            column.IsHidden = info.IsHidden;

            if (info.State != ColumnVisibleState.AlwaysHidden)
            {
                if (info.State == ColumnVisibleState.AlwaysVisible)
                {
                    alwaysVisibleColumns.Add(column);
                }

                string prefix = string.Empty;
                foreach (var row in grid.StackedHeaderRows)
                {
                    foreach (var stackedColumn in row.StackedColumns)
                    {
                        var childs = stackedColumn.ChildColumns.Split(",", StringSplitOptions.TrimEntries);
                        if (childs.Contains(column.MappingName))
                        {
                            prefix += stackedColumn.HeaderText + ": ";
                            break;
                        }
                    }
                }
                
                var item = new MenuItemModel()
                {
                    Header = prefix + column.HeaderText,
                    IsChecked = !column.IsHidden,
                    IsEnabled = !alwaysVisibleColumns.Contains(column),
                    Tag = column,
                    PlacementTarget = this
                };

                VisibleColumnsMenuItems.Add(item);
            }
        }

        RefreshDataSource();

        isLoaded = true;
    }

    [RelayCommand(CanExecute = nameof(CanCopyRow))]
    private void CopyRow()
    {
        if (SelectedItem is not T row)
        {
            return;
        }

        try
        {
            if (CheckCopyRow(row))
            {
                using var conn = CurrentDatabase.OpenConnection();
                using var transaction = conn.BeginTransaction();

                try
                {
                    if (conn.Copy(row, out var copyRow, transaction))
                    {
                        var copy = AddRow(conn, ((T)copyRow).Id);
                        if (copy != null)
                        {
                            OnCopyNestedRows(conn, row, copy, transaction);
                        }

                        transaction.Commit();

                        var type = GetEditorViewType();
                        if (copy != null && type != null)
                        {
                            if (MessageBox.Show("Открыть окно для редактрования?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(type, copy));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool CanCopyRow() => GetEditorViewType() != null;

    [RelayCommand]
    private void CreateBasedDocument(MenuItemModel parameter)
    {
        if (parameter.Tag is CreatingBasedContext context && SelectedItem is DocumentInfo document)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(context.EditorType) { BasedDocument = document });
        }
    }

    #endregion

    #region Receives

    public void Receive(EntityActionMessage message)
    {
        if (message.Action == MessageAction.Refresh && message.Destination == MessageDestination.List && message.ObjectId != Guid.Empty)
        {
            if (Owner != null && Owner.GetType().Name.Underscore() == message.EntityName && Owner.Id == message.ObjectId)
            {
                logger.LogInformation("A message was received about the need to update all tables that are dependent on {Name} with Id = {Id}", message.EntityName, message.ObjectId);
                RefreshDataSource();
            }

            return;
        }

        if (message.EntityName == typeof(T).Name.Underscore())
        {
            switch (message.Action)
            {
                case MessageAction.Add:
                    AddRow(message.ObjectId);
                    break;
                case MessageAction.Delete:
                    switch (message.Destination)
                    {
                        case MessageDestination.Object:
                            RemoveRow(message.ObjectId);
                            break;
                        case MessageDestination.List:
                            logger.LogInformation("A message was received about the need to update the {Name} table after deleting a record", message.EntityName);
                            RefreshDataSource();
                            break;
                    }

                    break;
                case MessageAction.Refresh:
                    switch (message.Destination)
                    {
                        case MessageDestination.Object:
                            RefreshRow(message.ObjectId);
                            break;
                        case MessageDestination.List:
                            logger.LogInformation("A message was received about the need to update the {Name} table", message.EntityName);
                            RefreshDataSource();
                            break;
                    }

                    break;
            }
        }
    }

    public void Receive(PageClosedMessage message)
    {
        if (message.Value == this || (Owner != null && message.Value is IEntityEditorViewModel model && model.DocumentInfo != null && model.DocumentInfo == Owner))
        {
            if (grid != null)
            {
                int i = 1;
                var list = new List<GroupSettings>();
                foreach (var item in grid.GroupColumnDescriptions)
                {
                    var grp = new GroupSettings()
                    {
                        Order = i++,
                        Name = (item.Converter is CustomGroupConverter converter) ? converter.Name : item.ColumnName,
                        Extended = item.Converter != null
                    };

                    list.Add(grp);
                }

                settings.Groups = list.Count == 0 ? null : list;
            }

            settings.SaveAsync(GetConfigFileName(), GetFilter());
        }
    }

    #endregion

    public virtual DocumentInfo? GetReportingDocument(Report report)
    {
        if (SelectedItem is DocumentInfo document)
        {
            return document;
        }

        return null;
    }

    public void RefreshDataSource()
    {
        logger.LogInformation("The {Name} table refresh has started", typeof(T).Name);

        if (Owner == null && IsDependent)
        {
            return;
        }

        try
        {
            using var conn = CurrentDatabase.OpenConnection();

            DataSource = new ObservableCollection<T>(GetData(conn));
            OnAfterRefreshDataSource(conn);

            logger.LogInformation("The refresh of the {Name} table has been stopped", typeof(T).Name);
        }
        catch (Exception e)
        {
            logger.LogInformation(e, "Refresh the {Name} table caused an exception", typeof(T).Name);
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public virtual Type? GetEditorViewType() => null;

    protected virtual void InitializeToolBar() { }

    protected virtual void RegisterReports() { }

    protected abstract bool GetSupportAccepting();

    protected string GetConfigFileName() => typeof(T).Name + (Owner == null ? string.Empty : "Nested");

    protected void RegisterReport(Guid id)
    {
        using var conn = CurrentDatabase.OpenConnection();
        var report = conn.QueryFirst<Report>("select * from report where id = :id", new { id });

        MenuItemModel menuItem = new()
        {
            Header = report.Title,
            Tag = report,
            PlacementTarget = this
        };

        reports.Add(menuItem);
    }

    protected virtual void LoadFilter(IConfigurationSection section) { }

    protected virtual object? GetFilter() => null;

    protected virtual MessageOptions GetEditorOptions() => new DocumentEditorMessageOptions(Owner) { CanEdit = CanEditSelected() };

    protected bool CanEditSelected()
    {
        if (SelectedItem is T item)
        {
            return CanEditSelected(item);
        }

        return true;
    }

    protected virtual bool CanEditSelected(T selectedItem)
    {
        return !selectedItem.Deleted;
    }

    protected virtual void OnCopyNestedRows(IDbConnection connection, T from, T to, IDbTransaction? transaction = null) { }

    /// <summary>
    /// Возвращает список записей с учётом фильтров установленных в функции <see cref="DefaultQuery(IDbConnection)"/>.
    /// Если параметр id не равен null, то осуществляется поиск записи с указанным идентификатором. При этом,
    /// если запись не найдена, генерируется исключение <see cref="RecordNotFoundException"/>.
    /// </summary>
    /// <param name="id">Идентификатор записи, которую необходимо получить.</param>
    /// <returns></returns>
    protected T GetDataById(Guid id)
    {
        using var conn = CurrentDatabase.OpenConnection();

        return GetDataById(conn, id);
    }

    protected T GetDataById(IDbConnection connection, Guid id)
    {
        var rows = GetData(connection, id);
        if (rows.Count == 0)
        {
            throw new RecordNotFoundException(id);
        }

        return rows[0];
    }

    protected virtual IReadOnlyList<T> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id)
            .Get<T>()
            .ToList();
    }

    protected Query DefaultQuery(IDbConnection conn, Guid? id = null, QueryParameters? parameters = null)
    {
        parameters ??= QueryParameters.Default;
        return ApplyFilters(MappingsQuery(SelectQuery(RequiredQuery(conn, parameters))))
            .When(id != null, q => q.Where($"{parameters.Alias}.id", id));
    }

    protected virtual Query SelectQuery(Query query) => query;

    protected virtual Query MappingsQuery(Query query) => query;

    protected virtual Query WipeQuery(Query query) => query;

    protected virtual Query FilterQuery(Query query) => throw new NotImplementedException();

    protected Query RequiredQuery(IDbConnection connection, QueryParameters parameters)
    {
        var query = connection.GetQuery<T>(parameters);

        query.When(
            Owner != null,
            q => q.Where($"{parameters.Alias}.{parameters.OwnerIdName}", Owner!.Id));

        if (parameters.IncludeDocumentsInfo && CurrentDatabase.HasPrivilege("document_refs", Privilege.Select) && typeof(T).IsAssignableTo(typeof(DocumentInfo)))
        {
            query = query
                .SelectRaw($"exists(select 1 from document_refs dr where dr.owner_id = {parameters.Alias}.id) as has_documents")
                .SelectRaw($"(exists (select 1 from document_refs dr where ((dr.owner_id = {parameters.Alias}.id) and (dr.thumbnail is not null)))) AS has_thumbnails");
            var grp = query.GetComponents<Column>("group");
            if (grp.Any() && grp.FirstOrDefault(x => x.Name == $"{parameters.Alias}.id") == null)
            {
                query = query.GroupBy($"{parameters.Alias}.id");
            }
        }

        return query;
    }

    protected virtual void OnAfterRefreshDataSource(IDbConnection connection) { }

    protected virtual void ConfigureColumn(IColumnInfo columnInfo) { }

    protected virtual bool CheckDeleteRow(T row)
    {
        if (!row.Deleted)
        {
            return MessageBox.Show("Вы действительно хотите пометить запись как удалённую?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        return true;
    }

    protected virtual bool CheckWipeRow(T row) => true;

    protected virtual bool CheckCopyRow(T row) => true;

    protected void ExecuteSqlById(string sql, T row)
    {
        try
        {
            using var conn = CurrentDatabase.OpenConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                conn.Execute(sql, new { row.Id }, transaction);

                transaction.Commit();

                if (DataSource != null)
                {
                    DataSource[DataSource.IndexOf(row)] = GetDataById(row.Id);
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected void ExecuteSystemOperation(T row, SystemOperation operation, bool value)
    {
        var sql = $"call execute_system_operation(:Id, '{operation.ToString().Underscore()}'::system_operation, {value}, '{typeof(T).Name.Underscore()}')";
        ExecuteSqlById(sql, row);
    }

    private void InitializeViewer()
    {
        RegisterReports();
        InitializeToolBar();
    }

    private Query ApplyFilters(Query query)
    {
        try
        {
            query.Where(q => FilterQuery(query));
        }
        catch 
        { 
        }

        return query;
    }

    private T? AddRow(Guid id)
    {
        using var conn = CurrentDatabase.OpenConnection();
        return AddRow(conn, id);
    }

    private T? AddRow(IDbConnection connection, Guid id)
    {
        if (DataSource != null)
        {
            try
            {
                T row = GetDataById(connection, id);
                DataSource.Add(row);

                return row;
            }
            catch (RecordNotFoundException)
            {
            }
            catch (Exception e)
            {
                MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        return default;
    }

    private void RemoveRow(Guid id)
    {
        if (DataSource != null)
        {
            var row = DataSource.FirstOrDefault(x => x.Id == id);
            if (row != null)
            {
                DataSource.Remove(row);
            }
        }
    }

    private void RefreshRow(Guid id)
    {
        if (DataSource != null)
        {
            var row = DataSource.FirstOrDefault(x => x.Id == id);
            try
            {
                var refreshDoc = GetDataById(id);
                if (row != null)
                {
                    DataSource[DataSource.IndexOf(row)] = refreshDoc;
                }
                else
                {
                    DataSource.Add(refreshDoc);
                }
            }
            catch (RecordNotFoundException)
            {
                if (row != null)
                {
                    DataSource.Remove(row);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void SetMarkedValue(bool mark)
    {
        if (SelectedItem is T row && CheckDeleteRow(row))
        {
            ExecuteSystemOperation(row, SystemOperation.Delete, mark);
        }
    }

    private void WipeCurrentRow()
    {
        if (SelectedItem is not T row)
        {
            return;
        }

        if (!row.Deleted)
        {
            MessageBox.Show("Запись не помечена на удаление. Операция прервана", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            if (CheckWipeRow(row))
            {
                try
                {
                    using var conn = CurrentDatabase.OpenConnection();
                    using var transaction = conn.BeginTransaction();

                    try
                    {
                        var sql = $"delete from {typeof(T).Name.Underscore()} where id = :Id";
                        conn.Execute(sql, row, transaction);

                        transaction.Commit();

                        DataSource?.Remove(row);
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void WipeMarkedRows()
    {
        try
        {
            using var conn = CurrentDatabase.OpenConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                var query = DefaultQuery(conn).WhereTrue("deleted");

                WipeQuery(query).Delete(transaction);

                transaction.Commit();

                RefreshDataSource();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    partial void OnOwnerChanged(DocumentInfo? value)
    {
        //RefreshDataSource();
    }
}
