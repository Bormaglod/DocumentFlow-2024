//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common.Data;
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

public abstract partial class BaseCalculationOperationViewModel<T> : DirectoryViewModel<T>
    where T : CalculationOperation
{
    private readonly ICalculationItemRepository<T> repoOperations = null!;

    public BaseCalculationOperationViewModel() { }

    public BaseCalculationOperationViewModel(IDatabase database, ICalculationItemRepository<T> repoOperations, IConfiguration configuration, ILogger<BaseCalculationOperationViewModel<T>> logger) 
        : base(database, configuration, logger) 
    { 
        this.repoOperations = repoOperations;
    }

    #region Commands

    [RelayCommand]
    private void ShowOperation()
    {
        if (SelectedItem is T op && op.Operation != null)
        {
            var editorType = Type.GetType($"DocumentFlow.Views.Editors.{typeof(T).Name.Remove(0, "Calculation".Length)}View");
            if (editorType != null)
            {
                WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(editorType, op.Operation));
            }
        }
    }

    [RelayCommand]
    private void ShowMaterial()
    {
        if (SelectedItem is T op && op.Material != null)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(typeof(MaterialView), op.Material));
        }
    }

    [RelayCommand]
    private void RecalculatePrices()
    {
        if (Owner is Calculation calculation)
        {
            repoOperations.RecalculatePrices(calculation);
            RefreshDataSource();
        }
    }

    #endregion

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.ItemName))
        {
            columnInfo.State = ColumnVisibleState.AlwaysVisible;
        }
    }

    protected override Query SelectQuery(Query query)
    {
        var using_operations = new Query("calculation_operation as co")
            .SelectRaw("array_agg(co.code)")
            .WhereRaw("t0.code = any (co.previous_operation)")
            .WhereColumns("co.owner_id", "=", "t0.owner_id");

        return base
            .SelectQuery(query)
            .Select(using_operations, "using_operations")
            .SelectRaw("round((3600 * t0.repeats)::numeric / op.production_rate, 1) as produced_time")
            .SelectRaw("t0.material_amount * t0.repeats as total_material")
            .MappingQuery<T>(x => x.Operation, alias: "op")
            .MappingQuery<T>(x => x.Equipment)
            .MappingQuery<T>(x => x.Tool)
            .MappingQuery<T>(x => x.Material);
    }

    protected override IReadOnlyList<T> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id, new QueryParameters() { FromOnly = true })
            .Get<T, Operation, Equipment, Equipment, Material>(
                map: (op, operation, equipment, tools, material) =>
                {
                    op.Operation = operation;
                    op.Equipment = equipment;
                    op.Tool = tools;
                    op.Material = material;
                    return op;
                })
            .ToList();
    }

    protected override void InitializeToolBar()
    {
        base.InitializeToolBar();

        ToolBarItems.AddButtons(this,
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Производственная операция", "robot") { Command = ShowOperationCommand },
            new ToolBarButtonModel("Используемый материал", "goods") { Command = ShowMaterialCommand },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Пересчитать стоимость", "calculate") { Command = RecalculatePricesCommand });
    }
}
