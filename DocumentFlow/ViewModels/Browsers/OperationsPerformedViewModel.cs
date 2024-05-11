//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public partial class OperationsPerformedViewModel : DocumentViewModel<OperationsPerformed>, ISelfTransientLifetime
{
    public OperationsPerformedViewModel() { }

    public OperationsPerformedViewModel(IDatabase database, IConfiguration configuration)
        : base(database, configuration)
    {
    }

    public override Type? GetEditorViewType() => typeof(Views.Editors.OperationsPerformedView);

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<OperationsPerformed>(x => x.Operation, joinType: JoinType.Inner)
            .MappingQuery<OperationsPerformed>(x => x.Employee)
            .MappingQuery<CalculationOperation>(x => x.Material)
            .MappingQuery<OperationsPerformed>(x => x.ReplacingMaterial)
            .MappingQuery<OperationsPerformed>(x => x.Lot)
            .MappingQuery<ProductionLot>(x => x.Order)
            .MappingQuery<ProductionLot>(x => x.Calculation, QuantityInformation.None)
            .MappingQuery<Calculation>(x => x.Goods);
    }

    protected override IReadOnlyList<OperationsPerformed> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id)
            .Get<OperationsPerformed, CalculationOperation, OurEmployee, Material, Material, ProductionLot, ProductionOrder, Goods>(
                (op, operation, emp, material, rep_material, lot, order, goods) =>
                {
                    operation.Material = material;

                    lot.Order = order;

                    op.Operation = operation;
                    op.Employee = emp;
                    op.ReplacingMaterial = rep_material;
                    op.Lot = lot;
                    op.Goods = goods;

                    return op;
                })
            .ToList();
    }

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        base.ConfigureColumn(columnInfo);
        if (Owner != null)
        {
            if (new string[] { "Lot.Order", "Lot", "Goods.Code", "Goods.ItemName" }.Contains(columnInfo.MappingName))
            {
                columnInfo.State = ColumnVisibleState.AlwaysHidden;
            }
        }
    }
}
