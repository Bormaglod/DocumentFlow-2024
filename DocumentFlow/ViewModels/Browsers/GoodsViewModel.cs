//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class GoodsViewModel : ProductViewModel<Goods>, ISelfTransientLifetime
{
    private readonly IGoodsRepository goodsRepository = null!;

    public GoodsViewModel() { }

    public GoodsViewModel(IDatabase database, IGoodsRepository goodsRepository, IConfiguration configuration) 
        : base(database, configuration) 
    { 
        this.goodsRepository = goodsRepository;
    }

    public override Type? GetEditorViewType() => typeof(Views.Editors.GoodsView);

    public override DocumentInfo? GetReportingDocument(Report report)
    {
        if (SelectedItem is Goods goods) 
        {
            return goods.Calculation;
        }

        return null;
    }

    protected override void RegisterReports()
    {
        base.RegisterReports();
        RegisterReport(Report.Specification);
        RegisterReport(Report.ProcessMap);
    }

    protected override Query SelectQuery(Query query)
    {
        query = base
            .SelectQuery(query)
            .MappingQuery<Goods>(x => x.Measurement)
            .MappingQuery<Goods>(x => x.Calculation);

        if (CurrentDatabase.HasPrivilege("calculation", Privilege.Select))
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

    protected override void OnCopyNestedRows(IDbConnection connection, Goods from, Goods to, IDbTransaction? transaction = null)
    {
        base.OnCopyNestedRows(connection, from, to, transaction);

        goodsRepository.CopyCalculation(connection, from, to, transaction);
    }
}
