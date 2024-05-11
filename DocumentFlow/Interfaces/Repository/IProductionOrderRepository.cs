//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IProductionOrderRepository : IDocumentRepository<ProductionOrder>
{
    /// <summary>
    /// Метод возвращает список изделий из заказа указанного в <paramref name="order"/>.
    /// </summary>
    /// <param name="order">Заказ на изготовление.</param>
    /// <returns>Список изделий из заказа указанного в параметре order.</returns>
    IReadOnlyList<Goods> GetProducts(ProductionOrder order);

    /// <summary>
    /// Метод возвращает список изделий из заказа указанного в <paramref name="order"/>.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="order">Заказ на изготовление.</param>
    /// <returns>Список изделий из заказа указанного в параметре order.</returns>
    IReadOnlyList<Goods> GetProducts(IDbConnection connection, ProductionOrder order);

    /// <summary>
    /// Метод возвращает список активных заказов, т.е. не помеченных на удаление, проведённых и не закрытых.
    /// К списку заказов будет добавлен заказ <paramref name="includingOrder"/> при условии, что он не равен null
    /// </summary>
    /// <param name="includingOrder"></param>
    /// <returns>Список активных заказов.</returns>
    IReadOnlyList<ProductionOrder> GetActive(ProductionOrder? includingOrder);

    /// <summary>
    /// Метод возвращает список активных заказов, т.е. не помеченных на удаление, проведённых и не закрытых.
    /// К списку заказов будет добавлен заказ <paramref name="includingOrder"/> при условии, что он не равен null
    /// </summary>
    /// <param name="includingOrder"></param>
    /// <param name="connection"></param>
    /// <returns>Список активных заказов.</returns>
    IReadOnlyList<ProductionOrder> GetActive(IDbConnection connection, ProductionOrder? includingOrder);

    IList<ProductionOrderPrice> GetContent(ProductionOrder order);
    IList<ProductionOrderPrice> GetContent(IDbConnection connection, ProductionOrder order);
    void CopyContent(ProductionOrder orderFrom, ProductionOrder orderTo);
    void CopyContent(IDbConnection connection, ProductionOrder orderFrom, ProductionOrder orderTo, IDbTransaction? transaction);
}
