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
using Microsoft.Extensions.Logging;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public class FinishedGoodsViewModel : DocumentViewModel<FinishedGoods>, ISelfTransientLifetime
{
    public FinishedGoodsViewModel() { }

    public FinishedGoodsViewModel(IDatabase database, IConfiguration configuration, ILogger<FinishedGoodsViewModel> logger)
        : base(database, configuration, logger)
    {
    }

    public override Type? GetEditorViewType() => typeof(Views.Editors.FinishedGoodsView);

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<FinishedGoods>(x => x.Lot)
            .MappingQuery<ProductionLot>(x => x.Calculation, joinType: JoinType.Inner)
            .MappingQuery<Calculation>(x => x.Goods, joinType: JoinType.Inner)
            .MappingQuery<Goods>(x => x.Measurement);
    }

    protected override IReadOnlyList<FinishedGoods> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id)
            .Get<FinishedGoods, ProductionLot, Calculation, Goods, Measurement>(
                (fg, lot, calc, goods, measurement) =>
                {
                    goods.Measurement = measurement;
                    calc.Goods = goods;
                    lot.Calculation = calc;
                    fg.Lot = lot;

                    return fg;
                })
            .ToList();
    }

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        base.ConfigureColumn(columnInfo);
        if (columnInfo.MappingName == nameof(FinishedGoods.Lot))
        {
            columnInfo.State = ColumnVisibleState.AlwaysHidden;
        }

        if (Owner != null)
        {
            if (new string[] { "Lot.DocumentDate", "Lot.DocumentNumber" }.Contains(columnInfo.MappingName))
            {
                columnInfo.State = ColumnVisibleState.AlwaysHidden;
            }
        }
    }
}
