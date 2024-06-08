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

public partial class ProductionOrderViewModel : DocumentEditorViewModel<ProductionOrder>, ISelfTransientLifetime
{
    private readonly IProductionOrderRepository requestRepository = null!;
    private readonly IContractorRepository contractorRepository = null!;

    [ObservableProperty]
    private Contractor? contractor;

    [ObservableProperty]
    private Contract? contract;

    [ObservableProperty]
    private DependentCollection<ProductionOrderPrice>? products;

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

    [RelayCommand]
    private void AddProduct()
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

    [RelayCommand]
    private void EditProduct()
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

    [RelayCommand]
    private void DeleteProduct()
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
    private void CopyProduct()
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

        Contractors = contractorRepository.GetCustomers();

        if (entity != null)
        {
            Products = new DependentCollection<ProductionOrderPrice>(entity, requestRepository.GetContent(connection, entity));
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

    protected override void UpdateDependents(IDbConnection connection, ProductionOrder order, IDbTransaction? transaction = null)
    {
        if (Products != null)
        {
            Products.Owner = order;
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
}
