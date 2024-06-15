//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DocumentFlow.Common;
using DocumentFlow.Common.Collections;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;
using System.Windows;

namespace DocumentFlow.ViewModels.Editors;

public partial class WaybillSaleViewModel : DocumentEditorViewModel<WaybillSale>, ISelfTransientLifetime
{
    private readonly IWaybillSaleRepository waybillRepository = null!;
    private readonly IContractorRepository contractorRepository = null!;

    [ObservableProperty]
    private Contractor? contractor;

    [ObservableProperty]
    private Contract? contract;

    [ObservableProperty]
    private DependentCollection<WaybillSalePrice>? products;

    [ObservableProperty]
    private WaybillSalePrice? productSelected;

    [ObservableProperty]
    private IEnumerable<Contractor>? contractors;

    [ObservableProperty]
    private IEnumerable<Contract>? contracts;

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

    [ObservableProperty]
    private bool showInvoice;

    [ObservableProperty]
    private bool showUpd;

    public WaybillSaleViewModel() { }

    public WaybillSaleViewModel(
        IWaybillSaleRepository waybillRepository,
        IContractorRepository contractorRepository,
        IOrganizationRepository organizationRepository) : base(organizationRepository)
    {
        this.waybillRepository = waybillRepository;
        this.contractorRepository = contractorRepository;
    }

    #region Commands

    [RelayCommand]
    private void AddMaterial()
    {
        if (Products == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new ProductWindow()
        {
            Contractor = Contractor,
            Contract = Contract
        };

        if (dialog.Create<WaybillSalePrice>(out var product))
        {
            Products.Add(product);
        }
    }

    [RelayCommand]
    private void EditMaterial()
    {
        if (ProductSelected == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new ProductWindow()
        {
            Contractor = Contractor,
            Contract = Contract
        };

        dialog.Edit(ProductSelected);
    }

    [RelayCommand]
    private void DeleteMaterial()
    {
        if (Products != null && ProductSelected != null)
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            Products.Remove(ProductSelected);
        }
    }

    [RelayCommand]
    private void CopyMaterial()
    {
        if (Products == null || ProductSelected == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new ProductWindow()
        {
            Contractor = Contractor,
            Contract = Contract
        };

        if (dialog.CreateFrom(ProductSelected, out var product))
        {
            Products.Add(product);
        }
    }

    #endregion

    protected override string GetStandardHeader() => "Реализация";

    protected override void RaiseAfterLoadDocument(WaybillSale entity)
    {
        DocumentNumber = entity.DocumentNumber;
        DocumentDate = entity.DocumentDate;
        Organization = entity.Organization;

        Contractor = entity.Contractor;
        Contract = entity.Contract;
        WaybillNumber = entity.WaybillNumber;
        WaybillDate = entity.WaybillDate;
        InvoiceNumber = entity.InvoiceNumber;
        InvoiceDate = entity.InvoiceDate;
        Upd = entity.Upd;
    }

    protected override void UpdateEntity(WaybillSale entity)
    {
        entity.DocumentNumber = DocumentNumber;
        entity.DocumentDate = DocumentDate;
        entity.Organization = Organization;
        entity.Contractor = Contractor;
        entity.Contract = Contract;
        entity.WaybillDate = WaybillDate;
        entity.WaybillNumber = WaybillNumber;
        entity.InvoiceDate = InvoiceDate;
        entity.InvoiceNumber = InvoiceNumber;
        entity.Upd = Upd;
    }

    protected override IEnumerable<short> DisabledStates() => [State.Canceled];

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<WaybillSale>(x => x.State)
            .MappingQuery<WaybillSale>(x => x.Organization)
            .MappingQuery<WaybillSale>(x => x.Contractor)
            .MappingQuery<WaybillSale>(x => x.Contract);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<State, Organization, Contractor, Contract>(connection, (waybill, state, org, contractor, contract) =>
        {
            waybill.State = state;
            waybill.Organization = org;
            waybill.Contractor = contractor;
            waybill.Contract = contract;

            return waybill;
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, WaybillSale? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        Contractors = contractorRepository.GetSuppliers();

        if (entity != null)
        {
            Products = new DependentCollection<WaybillSalePrice>(entity, waybillRepository.GetContent(connection, entity));
        }
        else 
        {
            Products ??= [];
        }

        if (IsRefreshing && Contractor != null)
        {
            Contracts = contractorRepository.GetContracts(Contractor, ContractorType.Buyer);
        }
    }

    protected override void UpdateDependents(IDbConnection connection, WaybillSale waybill, IDbTransaction? transaction = null)
    {
        if (Products != null)
        {
            Products.Owner = waybill;
            connection.UpdateDependents(Products, transaction);
        }
    }

    partial void OnContractorChanged(Contractor? value)
    {
        if (Contract != null)
        {
            if (value == null || !value.ContainsContract(Contract))
            {
                Contract = null;
            }
        }

        if (value == null)
        {
            Contracts = null;
        }
        else
        {
            Contracts = contractorRepository.GetContracts(value, ContractorType.Buyer);
        }
    }

    partial void OnContractChanged(Contract? value)
    {
        if (value == null)
        {
            ShowInvoice = false;
            ShowUpd = false;
        }
        else
        {
            ShowInvoice = value.TaxPayer && !Upd;
            ShowUpd = value.TaxPayer;
        }
    }

    partial void OnUpdChanged(bool value)
    {
        ShowInvoice = (Contract?.TaxPayer ?? false) && !value;
    }
}
