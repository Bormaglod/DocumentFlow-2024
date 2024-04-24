//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IWaybillReceiptRepository : IDocumentRepository<WaybillReceipt>
{
    IList<WaybillReceiptPrice> GetContent(WaybillReceipt waybill);
    IList<WaybillReceiptPrice> GetContent(IDbConnection connection, WaybillReceipt waybill);
    void CopyContent(WaybillReceipt waybillFrom, WaybillReceipt waybillTo);
    void CopyContent(IDbConnection connection, WaybillReceipt waybillFrom, WaybillReceipt waybillTo, IDbTransaction? transaction);
}
