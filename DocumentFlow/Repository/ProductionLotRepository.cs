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

using System.Data;

namespace DocumentFlow.Repository;

public class ProductionLotRepository : DocumentRepository<ProductionLot>, IProductionLotRepository, ITransientLifetime
{
    public ProductionLotRepository(IDatabase database) : base(database) { }

    public IReadOnlyList<ProductionLot> GetActive(ProductionLot? lot)
    {
        using var conn = GetConnection();
        return GetActive(conn, lot);
    }

    public IReadOnlyList<ProductionLot> GetActive(IDbConnection connection, ProductionLot? lot)
    {
        return connection.GetQuery<ProductionLot>()
            .MappingQuery<ProductionLot>(x => x.Order)
            .MappingQuery<ProductionLot>(x => x.Calculation, joinType: JoinType.Inner)
            .MappingQuery<Calculation>(x => x.Goods, joinType: JoinType.Inner)
            .Where("t0.state_id", State.InActive)
            .When(lot != null, q => q
                .OrWhere("t0.id", lot!.Id))
            .Get<ProductionLot, ProductionOrder, Calculation, Goods>((lot, order, calc, goods) =>
            {
                calc.Goods = goods;

                lot.Order = order;
                lot.Calculation = calc;

                return lot;
            })
            .ToList();
    }
}
