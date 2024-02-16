//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

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
        return base
            .SelectQuery(query)
            .MappingQuery<CalculationOperation>(x => x.Equipment)
            .MappingQuery<CalculationOperation>(x => x.Tools)
            .MappingQuery<CalculationOperation>(x => x.Material);
    }

    protected override IReadOnlyList<CalculationOperation> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id)
            .Get<CalculationOperation, Equipment, Equipment, Material>(
                map: (op, equipment, tools, material) =>
                {
                    op.Equipment = equipment;
                    op.Tools = tools;
                    op.Material = material;
                    return op;
                })
            .ToList();
    }
}
