//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public class CalculationOperationViewModel : DirectoryViewModel<CalculationOperation>, ISelfTransientLifetime
{
    public CalculationOperationViewModel() { }

    public CalculationOperationViewModel(IDatabase database) : base(database) { }

    //public override Type? GetEditorViewType() => typeof(Views.Editors.ContractApplicationView);

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.ItemName))
        {
            columnInfo.AlwaysVisible = true;
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
            .MappingQuery<CalculationOperation>(x => x.Operation, "op")
            .MappingQuery<CalculationOperation>(x => x.Equipment)
            .MappingQuery<CalculationOperation>(x => x.Tools)
            .MappingQuery<CalculationOperation>(x => x.Material);
    }

    protected override IReadOnlyList<CalculationOperation> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id, new QueryParemeters() { FromOnly = true })
            .Get<CalculationOperation, Operation, Equipment, Equipment, Material>(
                map: (op, operation, equipment, tools, material) =>
                {
                    op.Operation = operation;
                    op.Equipment = equipment;
                    op.Tools = tools;
                    op.Material = material;
                    return op;
                })
            .ToList();
    }
}
