//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Common.Comparers;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extension;
using DocumentFlow.Common.Messages;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;
using DocumentFlow.Common.Exceptions;
using Humanizer;

using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using Syncfusion.Windows.Shared;

using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;

namespace DocumentFlow.ViewModels;

public abstract partial class EntityGridViewModel<T> : ObservableObject, IRecipient<EntityActionMessage>, IEntityGridViewModel
    where T : DocumentInfo
{
    private class ColumnInfo : IColumnInfo
    {
        public ColumnInfo(GridColumn gridColumn)
        {
            Width = gridColumn.Width;
            IsHidden = gridColumn.IsHidden;
            ColumnSizer = gridColumn.ColumnSizer;
            AlwaysVisible = false;
            MappingName = gridColumn.MappingName;
        }

        public string MappingName { get; }
        public GridLengthUnitType ColumnSizer { get; set; }
        public double Width { get; set; }
        public bool IsHidden { get; set; }
        public bool AlwaysVisible { get; set; }
    }

    private readonly IDatabase database;

    private IGridPageView? view;
    private readonly List<GridColumn> alwaysVisibleColumns = new();

    [ObservableProperty]
    private ObservableCollection<T>? dataSource;

    [ObservableProperty]
    private ObservableCollection<MenuItemModel> visibleColumnsMenuItems = new();

    [ObservableProperty]
    public ObservableCollection<MenuItemModel> documentMenuItems = new();

    [ObservableProperty]
    public ObservableCollection<MenuItemModel> creationBasedMenuItems = new();

    [ObservableProperty]
    private object? selectedItem;

    [ObservableProperty]
    private DocumentInfo? owner;

    [ObservableProperty]
    private bool availableNavigation = true;

    [ObservableProperty]
    private bool availableGrouping = true;

    [ObservableProperty]
    private SizeMode sizeMode = SizeMode.Normal;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    public EntityGridViewModel() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

    public EntityGridViewModel(IDatabase database)
    {
        this.database = database;

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

        WeakReferenceMessenger.Default.Register(this);
    }

    public ToolBarViewModel ToolBarItems { get; } = new();

    #region Commands

    #region PopulateListDocuments

    private ICommand? populateListDocuments;

    public ICommand PopulateListDocuments
    {
        get
        {
            populateListDocuments ??= new BaseCommand(OnPopulateListDocuments);
            return populateListDocuments;
        }
    }

    private void OnPopulateListDocuments(object parameter)
    {
        DocumentMenuItems.Clear();
        if (SelectedItem is T info)
        {
            try
            {
                using var conn = database.OpenConnection();
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

    #endregion

    #region Refresh

    private ICommand? refresh;

    public ICommand Refresh
    {
        get
        {
            refresh ??= new DelegateCommand(OnRefreshDataSource);
            return refresh;
        }
    }

    private void OnRefreshDataSource(object parameter) => RefreshDataSource();

    #endregion

    #region OpenDocument

    private ICommand? openDocument;

    public ICommand OpenDocument
    {
        get
        {
            openDocument ??= new DelegateCommand<MenuItemModel>(OnOpenDocument);
            return openDocument;
        }
    }

    private void OnOpenDocument(MenuItemModel parameter)
    {
        if (parameter.Tag is OpenDocumentContext context)
        {
            WeakReferenceMessenger.Default.Send(new OpenDocumentMessage(context));
        }
    }

    #endregion

    #region CreateRow

    private ICommand? createRow;

    public ICommand CreateRow
    {
        get
        {
            createRow ??= new DelegateCommand(OnCreateRow, CanCreateRow);
            return createRow;
        }
    }

    private void OnCreateRow(object parameter)
    {
        var type = GetEditorViewType();
        if (type != null)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(type) { Options = GetEditorOptions() });
        }
    }

    private bool CanCreateRow(object parameter) => GetEditorViewType() != null;

    #endregion

    #region EditCurrentRow

    private ICommand? editCurrentRow;

    public ICommand EditCurrentRow
    {
        get
        {
            editCurrentRow ??= new DelegateCommand(OnEditCurrentRow, CanEditCurrentRow);
            return editCurrentRow;
        }
    }

    protected virtual void OnEditCurrentRow(object parameter)
    {
        var type = GetEditorViewType();
        if (SelectedItem is DocumentInfo document && type != null)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(type, document) { Options = GetEditorOptions() });
        }
    }

    private bool CanEditCurrentRow(object parameter) => GetEditorViewType() != null;

    #endregion

    #region SelectCurrentRow

    private ICommand? selectCurrentRow;

    public ICommand SelectCurrentRow
    {
        get
        {
            selectCurrentRow ??= new DelegateCommand(OnSelectCurrentRow);
            return selectCurrentRow;
        }
    }

    protected virtual void OnSelectCurrentRow(object parameter) => OnEditCurrentRow(parameter);

    #endregion

    #region SwapMarkedRow

    private ICommand? swapMarkedRow;

    public ICommand SwapMarkedRow
    {
        get
        {
            swapMarkedRow ??= new DelegateCommand(OnSwapMarkedRow);
            return swapMarkedRow;
        }
    }

    private void OnSwapMarkedRow(object parameter)
    {
        if (SelectedItem is T row)
        {
            SetMarkedValue(!row.Deleted);
        }
    }

    #endregion

    #region WipeRows

    private ICommand? wipeRows;

    public ICommand WipeRows
    {
        get
        {
            wipeRows ??= new DelegateCommand(OnWipeRows);
            return wipeRows;
        }
    }

    private void OnWipeRows(object parameter)
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

    #endregion

    #region CopyRow

    private ICommand? copyRow;

    public ICommand CopyRow
    {
        get
        {
            copyRow ??= new DelegateCommand(OnCopyRow);
            return copyRow;
        }
    }

    private void OnCopyRow(object parameter)
    {
        if (SelectedItem is not T row)
        {
            return;
        }

        try
        {
            if (CheckCopyRow(row))
            {
                using var conn = database.OpenConnection();
                using var transaction = conn.BeginTransaction();

                try
                {
                    if (conn.Copy(row, out var copyRow, transaction))
                    {
                        transaction.Commit();
                        
                        var copy = AddRow(conn, ((T)copyRow).Id);

                        var type = GetEditorViewType();
                        if (copy != null && type != null)
                        {
                            OnCopyRow(copy);
                            if (MessageBox.Show("Открыть окно для редактрования?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(type, copy));
                            }
                        }
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    #endregion

    #region CreateBasedDocument

    private ICommand? createBasedDocument;

    public ICommand CreateBasedDocument
    {
        get
        {
            createBasedDocument ??= new DelegateCommand<MenuItemModel>(OnCreateBasedDocument);
            return createBasedDocument;
        }
    }

    private void OnCreateBasedDocument(MenuItemModel parameter)
    {
        if (parameter.Tag is CreatingBasedContext context && SelectedItem is DocumentInfo document)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(context.EditorType) { BasedDocument = document });
        }
    }

    #endregion

    #endregion

    public IGridPageView? View
    {
        get => view;
        set
        {
            if (view != value)
            {
                view = value;
                OnPageViewChanged();
            }
        }
    }

    #region Receives

    public void Receive(EntityActionMessage message)
    {
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
                            RefreshDataSource();
                            break;
                    }

                    break;
            }
        }
    }

    #endregion

    public void RefreshDataSource()
    {
        try
        {
            using var conn = database.OpenConnection();

            DataSource = new ObservableCollection<T>(GetData(conn));
            OnAfterRefreshDataSource();
        }
        catch (Exception e)
        {
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public virtual Type? GetEditorViewType() => null;

    protected virtual MessageOptions GetEditorOptions() => new DocumentEditorMessageOptions(Owner);

    /// <summary>
    /// Возвращает список записей с учётом фильтров установленных в функции <see cref="DefaultQuery(IDbConnection)"/>.
    /// Если параметр id не равен null, то осуществляется поиск записи с указанным идентификатором. При этом,
    /// если запись не найдена, генерируется исключение <see cref="RecordNotFoundException"/>.
    /// </summary>
    /// <param name="id">Идентификатор записи, которую необходимо получить.</param>
    /// <returns></returns>
    protected IReadOnlyList<T> GetData(Guid? id = null)
    {
        using var conn = database.OpenConnection();

        return GetData(conn, id);
    }

    protected virtual IReadOnlyList<T> GetData(IDbConnection connection, Guid? id = null)
    {
        var list = DefaultQuery(connection)
            .When(id != null, q => q.Where("t0.id", id))
            .Get<T>()
            .ToList();

        if (id != null && list.Count == 0)
        {
            throw new RecordNotFoundException(id);
        }

        return list;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="P"></typeparam>
    /// <param name="map"></param>
    /// <param name="referenceMap"></param>
    /// <returns></returns>
    protected IReadOnlyList<T> GetData<P>(IDbConnection connection, Func<T, P, T> map, Func<string, string>? referenceMap = null, Guid? id = null)
    {
        var query = MappingQuery<P>(DefaultQuery(connection), referenceMap)
            .When(id != null, q => q.Where("t0.id", id));

        var compiled = ((XQuery)query).QueryFactory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);
        var list = connection.Query(
            compiled.Sql,
            map, 
            parameters).ToList();

        if (id != null && list.Count == 0)
        {
            throw new RecordNotFoundException(id);
        }

        return list;
    }

    protected IReadOnlyList<T> GetData<P1, P2>(IDbConnection connection, Func<T, P1, P2, T> map, Func<string, string>? referenceMap = null, Guid? id = null)
    {
        var query = MappingQuery<P2>(MappingQuery<P1>(DefaultQuery(connection), referenceMap), referenceMap)
            .When(id != null, q => q.Where("t0.id", id));

        var compiled = ((XQuery)query).QueryFactory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);
        var list = connection.Query(
            compiled.Sql,
            map,
            parameters).ToList();

        if (id != null && list.Count == 0)
        {
            throw new RecordNotFoundException(id);
        }

        return list;
    }

    protected IReadOnlyList<T> GetData<P1, P2, P3>(IDbConnection connection, Func<T, P1, P2, P3, T> map, Func<string, string>? referenceMap = null, Guid? id = null)
    {
        var query = MappingQuery<P3>(MappingQuery<P2>(MappingQuery<P1>(DefaultQuery(connection), referenceMap), referenceMap), referenceMap)
            .When(id != null, q => q.Where("t0.id", id));

        var compiled = ((XQuery)query).QueryFactory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);
        var list = connection.Query(
            compiled.Sql,
            map,
            parameters).ToList();

        if (id != null && list.Count == 0)
        {
            throw new RecordNotFoundException(id);
        }

        return list;
    }

    protected Query DefaultQuery(IDbConnection conn) => SelectQuery(RequiredQuery(conn));

    protected virtual Query SelectQuery(Query query) => query;

    protected virtual Query WipeQuery(Query query) => query;

    protected Query GetQuery(IDbConnection conn)
    {
        var factory = new QueryFactory(conn, new PostgresCompiler());
        return factory.Query($"{typeof(T).Name.Underscore()} as t0");
    }

    protected Query RequiredQuery(IDbConnection connection)
    {
        var query = GetQuery(connection);

        if (query.Clauses.FirstOrDefault(c => c.Component == "select") == null)
        {
            query = query.Select("t0.*");
        }

        query.When(
            Owner != null,
            q => q.Where($"t0.owner_id", Owner!.Id));

        if (database.HasPrivilege("document_refs", Privilege.Select) && typeof(T).IsAssignableTo(typeof(DocumentInfo)))
        {
            query = query.SelectRaw("exists(select 1 from document_refs dr where dr.owner_id = t0.id) as has_documents");
            var grp = query.Clauses.Where(c => c.Component == "group").OfType<Column>();
            if (grp.Any() && grp.FirstOrDefault(x => x.Name == "t0.id") == null)
            {
                query = query.GroupBy("t0.id");
            }
        }

        return query;
    }

    protected virtual void OnAfterRefreshDataSource() { }

    protected virtual void OnPageViewChanged()
    {
        var grid = View?.DataGrid;
        if (grid == null)
        {
            return;
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

            if (info.AlwaysVisible)
            {
                alwaysVisibleColumns.Add(column);
            }

            var item = new MenuItemModel()
            {
                Header = column.HeaderText,
                IsChecked = !column.IsHidden,
                IsEnabled = !alwaysVisibleColumns.Contains(column),
                Tag = column,
                PlacementTarget = this
            };

            VisibleColumnsMenuItems.Add(item);
        }

        RefreshDataSource();
    }

    protected virtual void ConfigureColumn(IColumnInfo columnInfo) { }

    protected virtual bool CheckDeleteRow(T row)
    {
        return MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }

    protected virtual bool CheckWipeRow(T row) => true;

    protected virtual bool CheckCopyRow(T row) => true;

    private static Query MappingQuery<P>(Query query, Func<string, string>? referenceMap = null)
    {
        var table = typeof(P).Name.Underscore();

        string refName;
        if (referenceMap != null)
        {
            refName = referenceMap($"{table}_id");
        }
        else
        {
            refName = $"{table}_id";
        }

        return query
            .Select($"{table}.*")
            .LeftJoin(table, $"{table}.id", $"t0.{refName}");
    }

    private T? AddRow(Guid id)
    {
        using var conn = database.OpenConnection();
        return AddRow(conn, id);
    }

    private T? AddRow(IDbConnection connection, Guid id)
    {
        if (DataSource != null)
        {
            try
            {
                T row = GetData(connection, id).First();
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
                var refreshDoc = GetData(id).First();
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
            try
            {
                using var conn = database.OpenConnection();
                using var transaction = conn.BeginTransaction();

                try
                {
                    var sql = $"update {typeof(T).Name.Underscore()} set deleted = {mark} where id = :Id";
                    conn.Execute(sql, row, transaction);

                    transaction.Commit();

                    if (DataSource != null)
                    {
                        DataSource[DataSource.IndexOf(row)] = GetData(row.Id).First();
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
                    using var conn = database.OpenConnection();
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
            using var conn = database.OpenConnection();
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

    partial void OnOwnerChanged(DocumentInfo? oldValue, DocumentInfo? newValue)
    {
        if (oldValue?.Id != newValue?.Id) 
        {
            RefreshDataSource();
        }
    }
}
