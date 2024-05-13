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
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;

using Syncfusion.Windows.Shared;

using System.Data;
using System.Windows;
using System.Windows.Input;

namespace DocumentFlow.ViewModels;

public abstract partial class DocumentEditorViewModel<T> : EntityEditorViewModel<T>
    where T : BaseDocument, new()
{
    private readonly IOrganizationRepository organizationRepository = null!;

    [ObservableProperty]
    private int? documentNumber;

    [ObservableProperty]
    private DateTime documentDate = DateTime.Now;

    [ObservableProperty]
    private Organization? organization;

    [ObservableProperty]
    private IEnumerable<Organization>? organizations;

    [ObservableProperty]
    private bool enabledDocumentNumber;

    public DocumentEditorViewModel() { }

    public DocumentEditorViewModel(IOrganizationRepository organizationRepository) : base()
    {
        this.organizationRepository = organizationRepository;
    }

    #region Commands

    #region AcceptCommand

    private ICommand? acceptCommand;

    public ICommand AcceptCommand
    {
        get
        {
            acceptCommand ??= new DelegateCommand(OnAcceptCommand);
            return acceptCommand;
        }
    }

    private void OnAcceptCommand(object parameter) => OnAccept();

    #endregion

    #region AcceptAndCloseCommand

    private ICommand? acceptAndCloseCommand;

    public ICommand AcceptAndCloseCommand
    {
        get
        {
            acceptAndCloseCommand ??= new DelegateCommand(OnAcceptAndCloseCommand);
            return acceptAndCloseCommand;
        }
    }

    private void OnAcceptAndCloseCommand(object parameter)
    {
        OnAccept();
        if (View != null)
        {
            WeakReferenceMessenger.Default.Send(new RequestClosePageMessage(View));
        }
    }

    #endregion

    #endregion

    protected override void InitializeToolBar(IDatabase? database = null)
    {
        base.InitializeToolBar(database);

        ToolBarItems.AddButtons(this,
            new ToolBarButtonModel("Обновить", "sync") { Command = Refresh },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Сохранить", "save") { Command = Save },
            new ToolBarButtonModel("Сохранить и закрыть", "save-close") { Command = SaveAndClose },
            new ToolBarButtonModel("Провести", "document-accept") { Command = AcceptCommand },
            new ToolBarButtonModel("Провести и закрыть", "doc-accept-close") { Command = AcceptAndCloseCommand },
            new ToolBarSeparatorModel(),
            new ToolBarButtonComboModel("Печать", "print", Reports));
    }

    protected override void InitializeEntityCollections(IDbConnection connection, T? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);
        Organizations = organizationRepository.GetOrganizations(connection);
        Organization = Organizations.FirstOrDefault(x => x.DefaultOrg);
    }

    protected virtual IEnumerable<short> DisabledStates() => Array.Empty<short>();

    protected override void UpdateUIControls(T entity)
    {
        if (Enabled)
        {
            Enabled = entity.State == null || !DisabledStates().Contains(entity.State.Id);
            if (!Enabled)
            {
                EnabledDocumentNumber = false;
            }
        }
    }

    private string GetHeaderStringValue() => $"№ {(DocumentNumber.ToString() ?? "б/н")} от {DocumentDate:d}";

    private void OnAccept()
    {
        if (OnSave(false))
        {
            try
            {
                using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
                using var transaction = conn.BeginTransaction();

                try
                {
                    var table = EntityProperties.GetTableName(typeof(T));
                    var sql = $"call execute_system_operation(:Id, 'accept'::system_operation, true, '{table}')";
                    conn.Execute(sql, new { Id }, transaction);

                    transaction.Commit();

                    WeakReferenceMessenger.Default.Send(new EntityActionMessage(table, Id, MessageAction.Refresh));
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

    partial void OnDocumentNumberChanged(int? value)
    {
        UpdateHeader(GetHeaderStringValue());
        if (value == null)
        {
            EnabledDocumentNumber = false;
        }
        else
        {
            EnabledDocumentNumber = Enabled;
        }
    }

    partial void OnDocumentDateChanged(DateTime value) => UpdateHeader(GetHeaderStringValue());
}
