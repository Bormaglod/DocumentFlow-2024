//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IPurchaseRequestRepository : IDocumentRepository<PurchaseRequest>
{
    IList<PurchaseRequestPrice> GetContent(PurchaseRequest request);
    IList<PurchaseRequestPrice> GetContent(IDbConnection connection, PurchaseRequest request);
    void CopyContent(PurchaseRequest requestFrom, PurchaseRequest requestTo);
    void CopyContent(IDbConnection connection, PurchaseRequest requestFrom, PurchaseRequest requestTo, IDbTransaction? transaction);
}
