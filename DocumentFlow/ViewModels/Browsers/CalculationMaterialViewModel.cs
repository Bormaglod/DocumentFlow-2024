//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;
using DocumentFlow.Views.Editors;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public sealed partial class CalculationMaterialViewModel : DirectoryViewModel<CalculationMaterial>, ISelfTransientLifetime
{
    private readonly ICalculationMaterialRepository repoMaterials = null!;

    public CalculationMaterialViewModel() { }

    public CalculationMaterialViewModel(IDatabase database, ICalculationMaterialRepository repoMaterials, IConfiguration configuration, ILogger<CalculationMaterialViewModel> logger) 
        : base(database, configuration, logger) 
    { 
        this.repoMaterials = repoMaterials;
    }

    #region Commands

    [RelayCommand]
    private void ShowMaterial()
    {
        if (SelectedItem is CalculationMaterial op && op.Material != null)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(typeof(MaterialView), op.Material));
        }
    }

    [RelayCommand]
    private void RecalculateCount()
    {
        if (Owner is Calculation calculation)
        {
            repoMaterials.RecalculateCount(calculation);
            RefreshDataSource();
        }
    }

    [RelayCommand]
    private void RecalculatePrices()
    {
        if (Owner is Calculation calculation)
        {
            repoMaterials.RecalculatePrices(calculation);
            RefreshDataSource();
        }
    }

    #endregion

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(CalculationMaterial.Material))
        {
            columnInfo.State = ColumnVisibleState.AlwaysVisible;
        }
    }

    protected override void InitializeToolBar()
    {
        base.InitializeToolBar();

        ToolBarItems.AddButtons(this,
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Материал", "goods") { Command = ShowMaterialCommand },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Пересчитать количество", "sigma") { Command = RecalculateCountCommand },
            new ToolBarButtonModel("Пересчитать стоимость", "calculate") { Command = RecalculatePricesCommand });
    }

    protected override IReadOnlyList<CalculationMaterial> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id)
            .Get<CalculationMaterial, Material>(
                map: (op, material) =>
                {
                    op.Material = material;
                    return op;
                })
            .ToList();
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .SelectRaw("round(m.weight * t0.amount, 3) as weight")
            .MappingQuery<CalculationMaterial>(x => x.Material, alias: "m");
    }
}
