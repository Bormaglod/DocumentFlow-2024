//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;

using Syncfusion.Windows.Shared;

using System.Data;
using System.Windows;
using System.Windows.Input;

namespace DocumentFlow.ViewModels.Editors;

public partial class WaybillReceiptViewModel : DocumentEditorViewModel<WaybillReceipt>, ISelfTransientLifetime
{
    private readonly IWaybillReceiptRepository waybillRepository = null!;
    private readonly IContractorRepository contractorRepository = null!;
    private readonly IPurchaseRequestRepository requestRepository = null!;

    [ObservableProperty]
    private Contractor? contractor;

    [ObservableProperty]
    private Contract? contract;

    [ObservableProperty]
    private PurchaseRequest? purchaseRequest;

    [ObservableProperty]
    private IList<WaybillReceiptPrice>? materials;

    [ObservableProperty]
    private WaybillReceiptPrice? materialSelected;

    [ObservableProperty]
    private IEnumerable<Contractor>? contractors;

    [ObservableProperty]
    private IEnumerable<Contract>? contracts;

    [ObservableProperty]
    private IEnumerable<PurchaseRequest>? purchaseRequests;

    [ObservableProperty]
    private string? waybillNumber;

    [ObservableProperty]
    private DateTime? waybillDate = DateTime.Now;

    [ObservableProperty]
    private string? invoiceNumber;

    [ObservableProperty]
    private DateTime? invoiceDate = DateTime.Now;

    [ObservableProperty]
    private bool upd;

    public WaybillReceiptViewModel() { }

    public WaybillReceiptViewModel(
        IWaybillReceiptRepository waybillRepository,
        IPurchaseRequestRepository requestRepository,
        IContractorRepository contractorRepository,
        IOrganizationRepository organizationRepository) : base(organizationRepository)
    {
        this.waybillRepository = waybillRepository;
        this.requestRepository = requestRepository;
        this.contractorRepository = contractorRepository;
    }

    #region Commands

    #region AddMaterial

    private ICommand? addMaterial;

    public ICommand AddMaterial
    {
        get
        {
            addMaterial ??= new DelegateCommand(OnAddMaterial);
            return addMaterial;
        }
    }

    private void OnAddMaterial(object parameter)
    {
        if (Materials == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new ProductWindow()
        {
            Contract = Contract
        };

        if (dialog.Create<WaybillReceiptPrice>(out var product))
        {
            Materials.Add(product);
        }
    }

    #endregion

    #region EditMaterial

    private ICommand? editMaterial;

    public ICommand EditMaterial
    {
        get
        {
            editMaterial ??= new DelegateCommand(OnEditMaterial);
            return editMaterial;
        }
    }

    private void OnEditMaterial(object parameter)
    {
        if (MaterialSelected == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new ProductWindow()
        {
            Contract = Contract
        };

        dialog.Edit(MaterialSelected);
    }

    #endregion

    #region DeleteMaterial

    private ICommand? deleteMaterial;

    public ICommand DeleteMaterial
    {
        get
        {
            deleteMaterial ??= new DelegateCommand(OnDeleteMaterial);
            return deleteMaterial;
        }
    }

    private void OnDeleteMaterial(object parameter)
    {
        if (Materials != null && MaterialSelected != null)
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            Materials.Remove(MaterialSelected);
        }
    }

    #endregion

    #region CopyMaterial

    private ICommand? copyMaterial;

    public ICommand CopyMaterial
    {
        get
        {
            copyMaterial ??= new DelegateCommand(OnCopyMaterial);
            return copyMaterial;
        }
    }

    private void OnCopyMaterial(object parameter)
    {
        if (Materials == null || MaterialSelected == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new ProductWindow()
        {
            Contract = Contract
        };

        if (dialog.CreateFrom(MaterialSelected, out var product))
        {
            Materials.Add(product);
        }
    }

    #endregion

    #region PurchaseRequestSelected

    private ICommand? purchaseRequestSelected;

    public ICommand PurchaseRequestSelected
    {
        get
        {
            purchaseRequestSelected ??= new DelegateCommand(OnPurchaseRequestSelected);
            return purchaseRequestSelected;
        }
    }

    private void OnPurchaseRequestSelected(object parameter)
    {
        if (parameter is PurchaseRequest request)
        {
            Contract = request.Contract;
        }
    }

    #endregion

    #endregion

    protected override string GetStandardHeader() => "Поступление";

    protected override void RaiseAfterLoadDocument(WaybillReceipt entity)
    {
        DocumentNumber = entity.DocumentNumber;
        DocumentDate = entity.DocumentDate;
        Organization = entity.Organization;

        // Заявку надо записать раньше контрагента, т.к. она будет использована в OnContractorChanged
        PurchaseRequest = entity.PurchaseRequest;
        Contractor = entity.Contractor;
        Contract = entity.Contract;
        WaybillNumber = entity.WaybillNumber;
        WaybillDate = entity.WaybillDate;
        InvoiceNumber = entity.InvoiceNumber;
        InvoiceDate = entity.InvoiceDate;
        Upd = entity.Upd;
    }

    protected override void UpdateEntity(WaybillReceipt entity)
    {
        entity.DocumentNumber = DocumentNumber;
        entity.DocumentDate = DocumentDate;
        entity.Organization = Organization;
        entity.PurchaseRequest = PurchaseRequest;
        entity.Contractor = Contractor;
        entity.Contract = Contract;
        entity.WaybillDate = WaybillDate;
        entity.WaybillNumber = WaybillNumber;
        entity.InvoiceDate = InvoiceDate;
        entity.InvoiceNumber = InvoiceNumber;
        entity.Upd = Upd;
        
    }

    protected override IEnumerable<short> DisabledStates() => new[] { State.Canceled, State.Completed };

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<WaybillReceipt>(x => x.State)
            .MappingQuery<WaybillReceipt>(x => x.Organization)
            .MappingQuery<WaybillReceipt>(x => x.Contractor)
            .MappingQuery<WaybillReceipt>(x => x.Contract)
            .MappingQuery<WaybillReceipt>(x => x.PurchaseRequest);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<State, Organization, Contractor, Contract, PurchaseRequest>(connection, (waybill, state, org, contractor, contract, request) =>
        {
            waybill.State = state;
            waybill.Organization = org;
            waybill.Contractor = contractor;
            waybill.Contract = contract;
            waybill.PurchaseRequest = request;

            return waybill;
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, WaybillReceipt? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        Contractors = contractorRepository.GetSlim();

        if (entity != null)
        {
            Materials = waybillRepository.GetContent(connection, entity);
        }
    }

    protected override void UpdateDependents(IDbConnection connection, IDbTransaction? transaction = null)
    {
        if (Materials != null)
        {
            connection.UpdateDependents(Materials, transaction);
        }
    }

    partial void OnContractorChanged(Contractor? value)
    {
        if (value == null)
        {
            Contracts = null;
            PurchaseRequests = null;
        }
        else
        {
            Contracts = contractorRepository.GetContracts(value);
            PurchaseRequests = requestRepository.GetActive(value, PurchaseRequest);
        }
    }
}
