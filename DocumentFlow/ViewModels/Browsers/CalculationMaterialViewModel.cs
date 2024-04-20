//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;
using DocumentFlow.Views.Editors;

using Microsoft.Extensions.Configuration;

using SqlKata;

using Syncfusion.Windows.Shared;

using System.Data;
using System.Windows.Input;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class CalculationMaterialViewModel : DirectoryViewModel<CalculationMaterial>, ISelfTransientLifetime
{
    private readonly ICalculationMaterialRepository repoMaterials = null!;

    public CalculationMaterialViewModel() { }

    public CalculationMaterialViewModel(IDatabase database, ICalculationMaterialRepository repoMaterials, IConfiguration configuration) 
        : base(database, configuration) 
    { 
        this.repoMaterials = repoMaterials;
    }

    #region Commands

    #region ShowMaterial

    private ICommand? showMaterial;

    public ICommand ShowMaterial
    {
        get
        {
            showMaterial ??= new DelegateCommand(OnShowMaterial);
            return showMaterial;
        }
    }

    private void OnShowMaterial(object parameter)
    {
        if (SelectedItem is CalculationMaterial op && op.Material != null)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(typeof(MaterialView), op.Material));
        }
    }

    #endregion

    #region RecalculateCount

    private ICommand? recalculateCount;

    public ICommand RecalculateCount
    {
        get
        {
            recalculateCount ??= new DelegateCommand(OnRecalculateCount);
            return recalculateCount;
        }
    }

    private void OnRecalculateCount(object parameter)
    {
        if (Owner is Calculation calculation)
        {
            repoMaterials.RecalculateCount(calculation);
            RefreshDataSource();
        }
    }

    #endregion

    #region RecalculatePrices

    private ICommand? recalculatePrices;

    public ICommand RecalculatePrices
    {
        get
        {
            recalculatePrices ??= new DelegateCommand(OnRecalculatePrices);
            return recalculatePrices;
        }
    }

    private void OnRecalculatePrices(object parameter)
    {
        if (Owner is Calculation calculation)
        {
            repoMaterials.RecalculatePrices(calculation);
            RefreshDataSource();
        }
    }

    #endregion

    #endregion

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(CalculationMaterial.Material))
        {
            columnInfo.AlwaysVisible = true;
        }
    }

    protected override void InitializeToolBar()
    {
        base.InitializeToolBar();

        ToolBarItems.AddButtons(this,
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Материал", "goods") { Command = ShowMaterial },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Пересчитать количество", "sigma") { Command = RecalculateCount },
            new ToolBarButtonModel("Пересчитать стоимость", "calculate") { Command = RecalculatePrices });
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
