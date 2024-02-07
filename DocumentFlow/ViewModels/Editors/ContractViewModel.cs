//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using Dapper;

using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.Data;
using System.Windows.Documents;

namespace DocumentFlow.ViewModels.Editors;

public partial class ContractViewModel : DirectoryEditorViewModel<Contract>, ISelfTransientLifetime
{
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

    public ContractViewModel(IDatabase database) : base(database) { }

    protected override string GetStandardHeader() => "Договор";

    protected override void RaiseAfterLoadDocument(Contract entity)
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
        var orgId = (contract?.OrganizationId) ?? connection.QuerySingleOrDefault<Guid>("select id from organization where default_org");
        Employees = GetForeignDirectory<Employee>(connection, Owner?.Id);
        OurEmployees = GetForeignDirectory<OurEmployee>(connection, orgId);
    }

    protected override void Load()
    {
        Load<Employee, OurEmployee>((contract, emp, orgEmp) =>
        {
            contract.Signatory = emp;
            contract.OrgSignatory = orgEmp;
            return contract;
        },
        (refs) =>
        {
            return refs switch
            {
                "employee_id" => "signatory_id",
                "our_employee_id" => "org_signatory_id",
                _ => refs
            };
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