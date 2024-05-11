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

    /// <summary>
    /// Возвращает список активных заявок, т.е. состояние заявки не должно быть в одном из: "Не установлено", "Составлен", "Отменён", "Выполнен".
    /// К списку заявок будет добавлена заявка <paramref name="includingRequest"/> при условии, что она не равна null и контрагент этой
    /// заявки тот же самый, чтот и <paramref name="contractor"/>.
    /// </summary>
    /// <param name="contractor"></param>
    /// <param name="includingRequest"></param>
    /// <returns></returns>
    IList<PurchaseRequest> GetActive(Contractor contractor, PurchaseRequest? includingRequest);

    /// <summary>
    /// Возвращает список активных заявок, т.е. состояние заявки не должно быть в одном из: "Не установлено", "Составлен", "Отменён", "Выполнен".
    /// К списку заявок будет добавлена заявка <paramref name="includingRequest"/> при условии, что она не равна null и контрагент этой
    /// заявки тот же самый, чтот и <paramref name="contractor"/>.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="contractor"></param>
    /// <param name="includingRequest"></param>
    /// <returns></returns>
    IList<PurchaseRequest> GetActive(IDbConnection connection, Contractor contractor, PurchaseRequest? includingRequest);
}
