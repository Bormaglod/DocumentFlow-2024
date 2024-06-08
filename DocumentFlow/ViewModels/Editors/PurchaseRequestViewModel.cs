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

public partial class PurchaseRequestViewModel : DocumentEditorViewModel<PurchaseRequest>, ISelfTransientLifetime
{
    private readonly IPurchaseRequestRepository requestRepository = null!;
    private readonly IContractorRepository contractorRepository = null!;

    [ObservableProperty]
    private Contractor? contractor;

    [ObservableProperty]
    private Contract? contract;

    [ObservableProperty]
    private string? note;

    [ObservableProperty]
    private DependentCollection<PurchaseRequestPrice>? materials;

    [ObservableProperty]
    private PurchaseRequestPrice? materialSelected;

    [ObservableProperty]
    private IEnumerable<Contractor>? contractors;

    [ObservableProperty]
    private IEnumerable<Contract>? contracts;

    public PurchaseRequestViewModel() { }

    public PurchaseRequestViewModel(
        IPurchaseRequestRepository requestRepository, 
        IContractorRepository contractorRepository, 
        IOrganizationRepository organizationRepository) : base(organizationRepository)
    {
        this.requestRepository = requestRepository;
        this.contractorRepository = contractorRepository;
    }

    #region Commands

    [RelayCommand]
    private void AddMaterial()
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

        if (dialog.Create<PurchaseRequestPrice>(out var product))
        {
            Materials.Add(product);
        }
    }

    [RelayCommand]
    private void EditMaterial()
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

    [RelayCommand]
    private void DeleteMaterial()
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

    [RelayCommand]
    private void CopyMaterial()
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

    protected override string GetStandardHeader() => "Заявка";

    protected override void RaiseAfterLoadDocument(PurchaseRequest entity)
    {
        DocumentNumber = entity.DocumentNumber;
        DocumentDate = entity.DocumentDate;
        Organization = entity.Organization;
        Contractor = entity.Contractor;
        Contract = entity.Contract;
        Note = entity.Note;
    }

    protected override void UpdateEntity(PurchaseRequest entity)
    {
        entity.DocumentNumber = DocumentNumber;
        entity.DocumentDate = DocumentDate;
        entity.Organization = Organization;
        entity.Contractor = Contractor;
        entity.Contract = Contract;
        entity.Note = Note;
    }

    protected override IEnumerable<short> DisabledStates() => new[] { State.Canceled, State.Completed };

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<PurchaseRequest>(x => x.State)
            .MappingQuery<PurchaseRequest>(x => x.Organization)
            .MappingQuery<PurchaseRequest>(x => x.Contractor)
            .MappingQuery<PurchaseRequest>(x => x.Contract);
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

    protected override void InitializeEntityCollections(IDbConnection connection, PurchaseRequest? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        Contractors = contractorRepository.GetSuppliers();

        if (entity != null) 
        {
            Materials = new DependentCollection<PurchaseRequestPrice>(entity, requestRepository.GetContent(connection, entity));
        }
        else
        {
            Materials ??= [];
        }

        if (IsRefreshing && Contractor != null)
        {
            Contracts = contractorRepository.GetContracts(Contractor, ContractorType.Seller);
        }
    }

    protected override void UpdateDependents(IDbConnection connection, PurchaseRequest request, IDbTransaction? transaction = null)
    {
        if (Materials != null)
        {
            Materials.Owner = request;
            connection.UpdateDependents(Materials, transaction);
        }
    }

    protected override void RegisterReports()
    {
        RegisterReport(Report.PurchaseRequest);
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
            Contracts = contractorRepository.GetContracts(value, ContractorType.Seller);
        }
    }
}
