//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IWaybillSaleRepository : IDocumentRepository<WaybillSale>
{
    IList<WaybillSalePrice> GetContent(WaybillSale waybill);
    IList<WaybillSalePrice> GetContent(IDbConnection connection, WaybillSale waybill);
    void CopyContent(WaybillSale waybillFrom, WaybillSale waybillTo);
    void CopyContent(IDbConnection connection, WaybillSale waybillFrom, WaybillSale waybillTo, IDbTransaction? transaction);
}
