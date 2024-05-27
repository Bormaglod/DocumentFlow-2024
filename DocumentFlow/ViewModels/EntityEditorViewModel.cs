//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Messages.Options;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;

using Humanizer;

using SqlKata;
using SqlKata.Execution;

using Syncfusion.Windows.Shared;

using System.Data;
using System.Windows;
using System.Windows.Input;

namespace DocumentFlow.ViewModels;

public abstract partial class EntityEditorViewModel<T> : ObservableObject, IRecipient<EntityActionMessage>, IEntityEditorViewModel, IReport
    where T : DocumentInfo, new()
{
    private string? headerDetails;
    private EntityEditStatus entityEditStatus = EntityEditStatus.Created;
    private readonly List<MenuItemModel> reports = new();
    private bool isRefreshing = false;

    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private DocumentInfo? owner;

    [ObservableProperty]
    private string header = string.Empty;

    [ObservableProperty]
    private bool enabled = true;

    [ObservableProperty]
    private T? entity;

    [ObservableProperty]
    private int nestedWindowsHeight = 300;

    public EntityEditorViewModel()
    {
        UpdateHeader();
        InitializeEditor();

        WeakReferenceMessenger.Default.Register(this);
    }

    public ToolBarViewModel ToolBarItems { get; } = new();
    public IEditorPageView? View { get; set; }
    public DocumentInfo? DocumentInfo => Entity;
    public bool IsRefreshing => isRefreshing;

    protected EntityEditStatus Status => entityEditStatus;
    protected IEnumerable<MenuItemModel> Reports => reports;

    #region Commands

    #region Refresh

    private ICommand? refresh;

    public ICommand Refresh
    {
        get
        {
            refresh ??= new DelegateCommand(OnRefresh);
            return refresh;
        }
    }

    private void OnRefresh(object parameter)
    {
        isRefreshing = true;
        try
        {
            if (Id != Guid.Empty)
            {
                LoadEntity();
            }
            else
            {
                using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
                InitializeEntityCollections(conn);
            }
        }
        finally
        {
            isRefreshing = false;
        }
    }

    #endregion

    #region Save

    private ICommand? save;

    public ICommand Save
    {
        get
        {
            save ??= new DelegateCommand(OnSave);
            return save;
        }
    }

    private void OnSave(object parameter) => OnSave();

    #endregion

    #region SaveAndClose

    private ICommand? saveAndClose;

    public ICommand SaveAndClose
    {
        get
        {
            saveAndClose ??= new DelegateCommand(OnSaveAndClose);
            return saveAndClose;
        }
    }

    private void OnSaveAndClose(object parameter)
    {
        OnSave();
        if (View != null)
        {
            WeakReferenceMessenger.Default.Send(new RequestClosePageMessage(View));
        }
    }

    #endregion

    #endregion

    #region Receives

    public void Receive(EntityActionMessage message)
    {
        if (message.EntityName == typeof(T).Name.Underscore() && message.Action == MessageAction.Refresh && message.Destination == MessageDestination.Object)
        {
            LoadDocument(message.ObjectId);
        }
    }

    #endregion

    public void LoadDocument(Guid id, MessageOptions? options = null)
    {
        Id = id;

        if (options != null)
        {
            SetOptions(options);
        }

        LoadEntity();
    }

    public void CreateDocument(MessageOptions? options)
    {
        if (options != null)
        {
            SetOptions(options);
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
        InitializeEntityCollections(conn);
    }

    public virtual DocumentInfo? GetReportingDocument(Report report) => Entity;

    protected virtual void SetOptions(MessageOptions options)
    {
        if (options is DocumentEditorMessageOptions documentOptions)
        {
            Owner = documentOptions.Owner;
            Enabled = documentOptions.CanEdit;
        }
    }

    protected virtual void InitializeToolBar(IDatabase? database = null) { }

    /// <summary>
    /// Метод предназначен для создания и заполнения коллекций используемых в данной модели. Этот методы вызывается
    /// после загрузки данных документа (до заполнения свойств модели, т.е. перед вызовом метода <see cref="RaiseAfterLoadDocument(T)"/>. 
    /// Параметр <paramref name="entity"/> при этом содержит загруженный объект. Либо этот метод вызывается при создании документа - тогда
    /// параметр <paramref name="entity"/> равен null. Кроме того - при вызове команды <see cref="Refresh"/>, если документ создан.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="entity"></param>
    /// <param name="isRefresh"></param>
    protected virtual void InitializeEntityCollections(IDbConnection connection, T? entity = null) { }

    protected virtual void UpdateUIControls(T entity) { }

    protected virtual void RegisterReports() { }

    protected void RegisterReport(Guid id)
    {
        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
        var report = conn.QueryFirst<Report>("select * from report where id = :id", new { id });

        MenuItemModel menuItem = new()
        {
            Header = report.Title,
            Tag = report,
            PlacementTarget = this
        };

        reports.Add(menuItem);
    }

    private void InitializeEditor()
    {
        RegisterReports();
        InitializeToolBar();
    }

    private void RaiseAfterLoad(IDbConnection connection, T entity)
    {
        InitializeEntityCollections(connection, entity);
        RaiseAfterLoadDocument(entity);
        UpdateUIControls(entity);
    }

    protected void LoadEntity()
    {
        try
        {
            entityEditStatus = EntityEditStatus.Loading;

            using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
            Load(conn);

            if (Entity != null)
            {
                RaiseAfterLoad(conn, Entity);
            }

            entityEditStatus = EntityEditStatus.Loaded;

        }
        catch (Exception e)
        {
            entityEditStatus = EntityEditStatus.Error;
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected virtual void Load(IDbConnection connection)
    {
        Entity = DefaultQuery(connection).First<T>();
    }

    protected void Load<P>(IDbConnection connection, Func<T, P, T> map, QueryParameters? parameters = null)
    {
        PrepareQuery(connection, out var compiled, out var sqlParameters, parameters);
        Entity = connection.Query(
            compiled.Sql,
            map,
            sqlParameters).First();
    }

    protected void Load<P1, P2>(IDbConnection connection, Func<T, P1, P2, T> map, QueryParameters? parameters = null)
    {
        PrepareQuery(connection, out var compiled, out var sqlParameters, parameters);
        Entity = connection.Query(
            compiled.Sql,
            map,
            sqlParameters).First();
    }

    protected void Load<P1, P2, P3>(IDbConnection connection, Func<T, P1, P2, P3, T> map, QueryParameters? parameters = null)
    {
        PrepareQuery(connection, out var compiled, out var sqlParameters, parameters);
        Entity = connection.Query(
            compiled.Sql,
            map,
            sqlParameters).First();
    }

    protected void Load<P1, P2, P3, P4>(IDbConnection connection, Func<T, P1, P2, P3, P4, T> map, QueryParameters? parameters = null)
    {
        PrepareQuery(connection, out var compiled, out var sqlParameters, parameters);
        Entity = connection.Query(
            compiled.Sql,
            map,
            sqlParameters).First();
    }

    protected void Load<P1, P2, P3, P4, P5>(IDbConnection connection, Func<T, P1, P2, P3, P4, P5, T> map, QueryParameters? parameters = null)
    {
        PrepareQuery(connection, out var compiled, out var sqlParameters, parameters);
        Entity = connection.Query(
            compiled.Sql,
            map,
            sqlParameters).First();
    }

    protected void Load<P1, P2, P3, P4, P5, P6>(IDbConnection connection, Func<T, P1, P2, P3, P4, P5, P6, T> map, QueryParameters? parameters = null)
    {
        PrepareQuery(connection, out var compiled, out var sqlParameters, parameters);
        Entity = connection.Query(
            compiled.Sql,
            map,
            sqlParameters).First();
    }

    protected void Load<P1, P2, P3, P4, P5, P6, P7>(IDbConnection connection, Func<T, P1, P2, P3, P4, P5, P6, P7, T> map, QueryParameters? parameters = null)
    {
        var query = DefaultQuery(connection, parameters);
        Entity = query.Get(map).First();
    }

    protected abstract string GetStandardHeader();

    protected abstract void RaiseAfterLoadDocument(T entity);

    protected abstract void UpdateEntity(T entity);

    protected virtual void UpdateDependents(IDbConnection connection, T entity, IDbTransaction? transaction = null) { }

    protected virtual Query SelectQuery(Query query) => query;

    protected void UpdateHeader(string details)
    {
        string readOnlyText = Enabled ? string.Empty : " (только для чтения)";

        if (Id == Guid.Empty)
        {
            if (string.IsNullOrEmpty(details))
            {
                Header = $"{GetStandardHeader()} (новый)";
            }
            else
            {
                Header = $"{GetStandardHeader()} - {details} (новый)";
            }
        }
        else
        {
            Header = $"{GetStandardHeader()} - {details}{readOnlyText}";
        }

        if (!string.IsNullOrEmpty(details))
        {
            headerDetails = details;
        }
    }

    protected bool OnSave(bool sendNotify = true)
    {
        MessageAction action = MessageAction.None;

        Entity ??= new();
        Entity.OwnerId = Owner?.Id;

        UpdateEntity(Entity);

        try
        {
            using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                if (Entity.Id == Guid.Empty)
                {
                    conn.Insert(Entity, transaction);
                    action = MessageAction.Add;
                }
                else
                {
                    conn.Update(Entity, transaction);
                    action = MessageAction.Refresh;
                }

                UpdateDependents(conn, Entity, transaction);

                transaction.Commit();

                Id = Entity.Id;

                LoadEntity();
                UpdateHeader();

                if (Id != Guid.Empty && sendNotify)
                {
                    WeakReferenceMessenger.Default.Send(new EntityActionMessage(EntityProperties.GetTableName(typeof(T)), Id, action));
                }
            }
            catch
            {
                if (action == MessageAction.Add)
                {
                    Entity.Id = Guid.Empty;
                }

                transaction.Rollback();
                throw;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        return true;
    }

    private void UpdateHeader() => UpdateHeader(headerDetails ?? string.Empty);

    private void PrepareQuery(IDbConnection connection, out SqlResult compiled, out DynamicParameters sqlParameters, QueryParameters? parameters = null)
    {
        var query = DefaultQuery(connection, parameters);
        compiled = ((XQuery)query).QueryFactory.Compiler.Compile(query);
        sqlParameters = new DynamicParameters(compiled.NamedBindings);
    }

    private Query DefaultQuery(IDbConnection connection, QueryParameters? parameters = null)
    {
        parameters ??= QueryParameters.Default;
        var query = SelectQuery(
            connection.GetQuery<T>(parameters)
                .Select("u1.name as user_created")
                .Select("u2.name as user_updated")
                .Join("user_alias as u1", $"{parameters.Alias}.user_created_id", "u1.id")
                .Join("user_alias as u2", $"{parameters.Alias}.user_updated_id", "u2.id")
                .Where($"{parameters.Alias}.id", Id));
        return query;
    }

    partial void OnHeaderChanging(string value)
    {
        if (View != null)
        {
            WeakReferenceMessenger.Default.Send(new EditorPageHeaderChangedMessage(View, value));
        }
    }
}
