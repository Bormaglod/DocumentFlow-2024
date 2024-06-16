//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using Dapper;

using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public partial class ContractViewModel : DirectoryEditorViewModel<Contract>, ISelfTransientLifetime
{
    private readonly IEmployeeRepository employeeRepository = null!;
    private readonly IOurEmployeeRepository ourEmployeeRepository = null!;

    [ObservableProperty]
    private ContractorType contractorType;

    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private DateTime dateContract;

    [ObservableProperty]
    private DateTime dateStart;

    [ObservableProperty]
    private DateTime? dateEnd;

    [ObservableProperty]
    private bool taxPayer;

    [ObservableProperty]
    private bool isDefault;

    [ObservableProperty]
    private long? paymentPeriod = null;

    [ObservableProperty]
    private Employee? signatory;

    [ObservableProperty]
    private OurEmployee? orgSignatory;

    [ObservableProperty]
    private IEnumerable<Employee>? employees;

    [ObservableProperty]
    private IEnumerable<OurEmployee>? ourEmployees;

    public ContractViewModel() { }

    public ContractViewModel(IEmployeeRepository employeeRepository, IOurEmployeeRepository ourEmployeeRepository) : base()
    {
        this.employeeRepository = employeeRepository;
        this.ourEmployeeRepository = ourEmployeeRepository;
    }

    protected override string GetStandardHeader() => "Договор";

    protected override void DoAfterLoadDocument(Contract entity)
    {
        ParentId = entity.ParentId;
        Code = entity.Code;
        ItemName = entity.ItemName;
        ContractorType = entity.ContractorType;
        DateContract = entity.DocumentDate;
        DateStart = entity.DateStart;
        DateEnd = entity.DateEnd;
        TaxPayer = entity.TaxPayer;
        IsDefault = entity.IsDefault;
        PaymentPeriod = entity.PaymentPeriod;
        Signatory = entity.Signatory;
        OrgSignatory = entity.OrgSignatory;
    }

    protected override void UpdateEntity(Contract entity)
    {
        entity.ParentId = ParentId;
        entity.Code = Code;
        entity.ItemName = ItemName;
        entity.ContractorType = ContractorType;
        entity.DocumentDate = DateContract;
        entity.DateStart = DateStart;
        entity.DateEnd = DateEnd;
        entity.TaxPayer = TaxPayer;
        entity.IsDefault = IsDefault;
        entity.PaymentPeriod = Convert.ToInt16(PaymentPeriod);
        entity.Signatory = Signatory;
        entity.OrgSignatory = OrgSignatory;
    }

    protected override void InitializeEntityCollections(IDbConnection connection, Contract? contract)
    {
        base.InitializeEntityCollections(connection, contract);
        if (Owner is Contractor contractor)
        {
            Employees = employeeRepository.GetEmployees(connection, contractor);
        }

        OurEmployees = ourEmployeeRepository.GetEmployees(connection);
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<Contract>(x => x.Signatory)
            .MappingQuery<Contract>(x => x.OrgSignatory);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<Employee, OurEmployee>(connection, (contract, emp, orgEmp) =>
        {
            contract.Signatory = emp;
            contract.OrgSignatory = orgEmp;
            return contract;
        });
    }

    partial void OnItemNameChanged(string? value)
    {
        UpdateHeader($"{value} {Code} от {DateContract:d}");
    }

    partial void OnCodeChanged(string value)
    {
        UpdateHeader($"{ItemName} {value} от {DateContract:d}");
    }

    partial void OnDateContractChanged(DateTime value)
    {
        UpdateHeader($"{ItemName} {Code} от {value:d}");
    }
}