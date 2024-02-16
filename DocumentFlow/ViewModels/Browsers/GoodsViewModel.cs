//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public class GoodsViewModel : ProductViewModel<Goods>, ISelfTransientLifetime
{
    private readonly IDatabase database;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    public GoodsViewModel() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

    public GoodsViewModel(IDatabase database) : base(database) 
    {
        this.database = database;
    }

    public override Type? GetEditorViewType() => typeof(Views.Editors.GoodsView);

    protected override Query SelectQuery(Query query)
    {
        query = base
            .SelectQuery(query)
            .MappingQuery<Goods>(x => x.Measurement)
            .MappingQuery<Goods>(x => x.Calculation);

        if (database.HasPrivilege("calculation", Privilege.Select))
        {
            Query pb = new Query("balance_product")
                .Select("reference_id")
                .SelectRaw("sum(amount) as product_balance")
                .GroupBy("reference_id");

            query = query
                .Select("pb.product_balance")
                .LeftJoin(
                    pb.As("pb"),
                    q => q.On("pb.reference_id", "t0.id"));
        }
        
        return query;
    }

    protected override IReadOnlyList<Goods> GetData(IDbConnection connection, Guid? id)
    {
        return DefaultQuery(connection, id)
            .Get<Goods, Measurement, Calculation>(
                map: (goods, measurement, calculation) =>
                {
                    goods.Measurement = measurement;
                    goods.Calculation = calculation;
                    return goods;
                })
            .ToList();
    }
}
