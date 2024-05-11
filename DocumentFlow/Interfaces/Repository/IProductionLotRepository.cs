//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IProductionLotRepository : IDocumentRepository<ProductionLot>
{
    IReadOnlyList<ProductionLot> GetActive(ProductionLot? lot);
    IReadOnlyList<ProductionLot> GetActive(IDbConnection connection, ProductionLot? lot);
}
