//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public partial class OperationsPerformedViewModel(
    IProductionLotRepository productionLotRepository,
    ICalculationRepository calculationRepository,
    IMaterialRepository materialRepository,
    IOurEmployeeRepository ourEmployeeRepository,
    IOrganizationRepository organizationRepository) : DocumentEditorViewModel<OperationsPerformed>(organizationRepository), ISelfTransientLifetime
{
    [ObservableProperty]
    private ProductionLot? lot;

    [ObservableProperty]
    private IEnumerable<ProductionLot>? lots;

    [ObservableProperty]
    private CalculationOperation? operation;

    [ObservableProperty]
    private IEnumerable<CalculationOperation>? operations;

    [ObservableProperty]
    private Material? replacingMaterial;

    [ObservableProperty]
    private IEnumerable<Material>? materials;

    [ObservableProperty]
    private bool skipMaterial;

    [ObservableProperty]
    private long quantity;

    [ObservableProperty]
    private Employee? employee;

    [ObservableProperty]
    private IEnumerable<Employee>? employees;

    [ObservableProperty]
    private decimal salary;

    [ObservableProperty]
    private bool? doubleRate;

    protected override string GetStandardHeader() => "Выполнение работы";

    protected override void DoAfterLoadDocument(OperationsPerformed entity)
    {
        DocumentNumber = entity.DocumentNumber;
        DocumentDate = entity.DocumentDate;
        Lot = entity.Lot;
        Operation = entity.Operation;
        ReplacingMaterial = entity.ReplacingMaterial;
        SkipMaterial = entity.SkipMaterial;
        Quantity = entity.Quantity;
        Employee = entity.Employee;
        Salary = entity.Salary;
        DoubleRate = entity.DoubleRate;
    }

    protected override void UpdateEntity(OperationsPerformed entity)
    {
        entity.DocumentNumber = DocumentNumber;
        entity.DocumentDate = DocumentDate;
        entity.Lot = Lot;
        entity.Operation = Operation;
        entity.ReplacingMaterial = ReplacingMaterial;
        entity.SkipMaterial = SkipMaterial;
        entity.Quantity = Quantity;
        entity.Employee = Employee;
        entity.Salary = Salary;
        entity.DoubleRate = DoubleRate;
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<OperationsPerformed>(x => x.Lot)
            .MappingQuery<ProductionLot>(x => x.Calculation)
            .MappingQuery<Calculation>(x => x.Goods)
            .MappingQuery<OperationsPerformed>(x => x.Operation, joinType: JoinType.Inner)
            .MappingQuery<OperationsPerformed>(x => x.Employee)
            .MappingQuery<CalculationOperation>(x => x.Material)
            .MappingQuery<OperationsPerformed>(x => x.ReplacingMaterial);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<ProductionLot, Calculation, Goods, CalculationOperation, OurEmployee, Material, Material>(connection,
            (op, lot, calculation, goods, operation, emp, material, rep_material) =>
            {
                calculation.Goods = goods;
                lot.Calculation = calculation;
                operation.Material = material;

                op.Lot = lot;
                op.Operation = operation;
                op.Employee = emp;
                op.ReplacingMaterial = rep_material;

                return op;
            });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, OperationsPerformed? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        Lots = productionLotRepository.GetActive(connection, entity?.Lot);

        if (entity?.Lot?.Calculation != null)
        {
            Operations = calculationRepository.GetOperations(connection, entity.Lot.Calculation, includeMaterialInfo: true);
        }

        Materials = materialRepository.GetSlim();
        Employees = ourEmployeeRepository.GetEmployees();
    }

    partial void OnLotChanged(ProductionLot? value)
    {
        Owner = value;
    }
}
