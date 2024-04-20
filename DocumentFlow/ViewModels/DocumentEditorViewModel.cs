//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Messages.Options;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;

using System.Data;

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

    protected override void InitializeToolBar(IDatabase? database = null)
    {
        base.InitializeToolBar(database);

        ToolBarItems.AddButtons(this,
            new ToolBarButtonModel("Обновить", "sync") { Command = Refresh },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Сохранить", "save") { Command = Save },
            new ToolBarButtonModel("Сохранить и закрыть", "save-close") { Command = SaveAndClose }); 
    }

    protected override void InitializeEntityCollections(IDbConnection connection, T? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);
        Organizations = organizationRepository.GetSlim(connection);
    }

    /*protected override void SetOptions(MessageOptions options)
    {
        base.SetOptions(options);
        
    }*/

    protected virtual IEnumerable<int> DisabledStates() => Array.Empty<int>();

    protected override void UpdateUIControls(T entity)
    {
        if (Enabled)
        {
            Enabled = entity.State != null && !DisabledStates().Contains(entity.State.Id);
            if (!Enabled)
            {
                EnabledDocumentNumber = false;
            }
        }
    }

    private string GetHeaderStringValue() => $"{GetStandardHeader()} № {(DocumentNumber.ToString() ?? "б/н")} от {DocumentDate:d}";

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
