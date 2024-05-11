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

public partial class ProductionOrderViewModel : DocumentEditorViewModel<ProductionOrder>, ISelfTransientLifetime
{
    private readonly IProductionOrderRepository requestRepository = null!;
    private readonly IContractorRepository contractorRepository = null!;

    [ObservableProperty]
    private Contractor? contractor;

    [ObservableProperty]
    private Contract? contract;

    [ObservableProperty]
    private IList<ProductionOrderPrice>? products;

    [ObservableProperty]
    private ProductionOrderPrice? productSelected;

    [ObservableProperty]
    private IEnumerable<Contractor>? contractors;

    [ObservableProperty]
    private IEnumerable<Contract>? contracts;

    public ProductionOrderViewModel(
        IProductionOrderRepository requestRepository,
        IContractorRepository contractorRepository,
        IOrganizationRepository organizationRepository) : base(organizationRepository)
    {
        this.requestRepository = requestRepository;
        this.contractorRepository = contractorRepository;
    }

    #region Commands

    #region AddProduct

    private ICommand? addProduct;

    public ICommand AddProduct
    {
        get
        {
            addProduct ??= new DelegateCommand(OnAddProduct);
            return addProduct;
        }
    }

    private void OnAddProduct(object parameter)
    {
        if (Products == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new ProductWindow()
        {
            Contract = Contract
        };

        if (dialog.Create<ProductionOrderPrice>(out var product))
        {
            Products.Add(product);
        }
    }

    #endregion

    #region EditProduct

    private ICommand? editProduct;

    public ICommand EditProduct
    {
        get
        {
            editProduct ??= new DelegateCommand(OnEditProduct);
            return editProduct;
        }
    }

    private void OnEditProduct(object parameter)
    {
        if (ProductSelected == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new ProductWindow()
        {
            Contract = Contract
        };

        dialog.Edit(ProductSelected);
    }

    #endregion

    #region DeleteProduct

    private ICommand? deleteProduct;

    public ICommand DeleteProduct
    {
        get
        {
            deleteProduct ??= new DelegateCommand(OnDeleteProduct);
            return deleteProduct;
        }
    }

    private void OnDeleteProduct(object parameter)
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

    #endregion

    #region CopyProduct

    private ICommand? copyProduct;

    public ICommand CopyProduct
    {
        get
        {
            copyProduct ??= new DelegateCommand(OnCopyProduct);
            return copyProduct;
        }
    }

    private void OnCopyProduct(object parameter)
    {
        if (Products == null || ProductSelected == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new ProductWindow()
        {
            Contract = Contract
        };

        if (dialog.CreateFrom(ProductSelected, out var product))
        {
            Products.Add(product);
        }
    }

    #endregion

    #endregion

    protected override string GetStandardHeader() => "Заказ";

    protected override void RaiseAfterLoadDocument(ProductionOrder entity)
    {
        DocumentNumber = entity.DocumentNumber;
        DocumentDate = entity.DocumentDate;
        Organization = entity.Organization;
        Contractor = entity.Contractor;
        Contract = entity.Contract;
    }

    protected override void UpdateEntity(ProductionOrder entity)
    {
        entity.DocumentNumber = DocumentNumber;
        entity.DocumentDate = DocumentDate;
        entity.Organization = Organization;
        entity.Contractor = Contractor;
        entity.Contract = Contract;
    }

    protected override IEnumerable<short> DisabledStates() => new[] { State.Canceled, State.Completed };

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<ProductionOrder>(x => x.State)
            .MappingQuery<ProductionOrder>(x => x.Organization)
            .MappingQuery<ProductionOrder>(x => x.Contractor)
            .MappingQuery<ProductionOrder>(x => x.Contract);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<State, Organization, Contractor, Contract>(connection, (request, state, org, contractor, contract) =>
        {
            request.State = state;
            request.Organization = org;
            request.Contractor = contractor;
            request.Contract = contract;
            return request;
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, ProductionOrder? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        Contractors = contractorRepository.GetSlim();

        if (entity != null)
        {
            Products = requestRepository.GetContent(connection, entity);
        }
    }

    protected override void UpdateDependents(IDbConnection connection, IDbTransaction? transaction = null)
    {
        if (Products != null)
        {
            connection.UpdateDependents(Products, transaction);
        }
    }

    partial void OnContractorChanged(Contractor? value)
    {
        if (value == null)
        {
            Contracts = null;
        }
        else
        {
            Contracts = contractorRepository.GetContracts(value);
        }
    }
}
