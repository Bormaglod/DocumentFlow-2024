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

public partial class CalculationDeductionViewModel : DirectoryEditorViewModel<CalculationDeduction>, ISelfTransientLifetime
{
    private readonly IDeductionRepository repoDeduction = null!;
    private readonly ICalculationOperationRepository repoOperations = null!;
    private readonly ICalculationMaterialRepository repoMaterials = null!;

    private bool isDeductionChanging;

    [ObservableProperty]
    private Deduction? deduction;

    [ObservableProperty]
    private decimal price;

    [ObservableProperty]
    private decimal value;

    [ObservableProperty]
    private decimal itemCost;

    [ObservableProperty]
    private IEnumerable<Deduction>? deductions;

    public CalculationDeductionViewModel() { }

    public CalculationDeductionViewModel(
        IDeductionRepository repoDeduction,
        ICalculationOperationRepository repoOperations,
        ICalculationMaterialRepository repoMaterials) : base()
    {
        this.repoDeduction = repoDeduction;
        this.repoOperations = repoOperations;
        this.repoMaterials = repoMaterials;
    }

    protected override string GetStandardHeader() => "Удержание";

    protected override void RaiseAfterLoadDocument(CalculationDeduction entity)
    {
        Deduction = entity.Deduction;
        Price = entity.Price;
        Value = entity.Value;
        ItemCost = entity.ItemCost;
    }

    protected override void UpdateEntity(CalculationDeduction entity)
    {
        entity.Deduction = Deduction;
        entity.Price = Price;
        entity.Value = Value;
        entity.ItemCost = ItemCost;
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<CalculationDeduction>(x => x.Deduction);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<Deduction>(connection, (calc, ded) =>
        {
            calc.Deduction = ded;
            return calc;
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, CalculationDeduction? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);
        Deductions = repoDeduction.GetSlim(connection);
    }

    partial void OnDeductionChanged(Deduction? value)
    {
        UpdateHeader(value?.Code ?? string.Empty);
        if (Owner is Calculation calculation && value != null)
        {
            isDeductionChanging = true;
            try
            {
                if (value.BaseDeduction == BaseDeduction.Salary)
                {
                    Price = repoOperations.GetSumItemCost(calculation);
                }
                else
                {
                    Price = repoMaterials.GetSumItemCost(calculation);
                }

                Value = value.Value;

                ItemCost = Price * Value / 100;
            }
            finally
            {
                isDeductionChanging = false;
            }
        }
    }

    partial void OnPriceChanged(decimal value)
    {
        if (!isDeductionChanging)
        {
            ItemCost = value * Value / 100;
        }
    }

    partial void OnValueChanged(decimal value)
    {
        if (!isDeductionChanging)
        {
            ItemCost = Price * value / 100;
        }
    }
}
