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
    /// <summary>
    /// Функция возвращает список партий находящихся в работе. К списку заказов будет добавлена партия <paramref name="lot"/> 
    /// (если она не равна null).
    /// </summary>
    /// <param name="lot"></param>
    /// <returns></returns>
    IReadOnlyList<ProductionLot> GetInProgress(ProductionLot? lot);

    /// <summary>
    /// Функция возвращает список партий находящихся в работе. К списку заказов будет добавлена партия <paramref name="lot"/> 
    /// (если она не равна null).
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="lot"></param>
    /// <returns></returns>
    IReadOnlyList<ProductionLot> GetInProgress(IDbConnection connection, ProductionLot? lot);

    /// <summary>
    /// Функция возвращает список партий находящихся в работе либо выполненных. Остаток изделий созданных в этой партии
    /// находящийся на складе не должен быть равен 0. К списку заказов будет добавлена партия <paramref name="lot"/> 
    /// (если она не равна null).
    /// </summary>
    /// <param name="lot"></param>
    /// <returns></returns>
    IReadOnlyList<ProductionLot> GetActive(ProductionLot? lot);

    /// <summary>
    /// Функция возвращает список партий находящихся в работе либо выполненных. Остаток изделий созданных в этой партии
    /// находящийся на складе не должен быть равен 0. К списку заказов будет добавлена партия <paramref name="lot"/> 
    /// (если она не равна null).
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="lot"></param>
    /// <returns></returns>
    IReadOnlyList<ProductionLot> GetActive(IDbConnection connection, ProductionLot? lot);

    /// <summary>
    /// Функция возвращает список партий находящихся в работе либо выполненных. Остаток изделий созданных в этой партии
    /// находящийся на складе не должен быть равен 0. Партия создана в рамках заказа контрагента <paramref name="contractor"/> и 
    /// изготовления <paramref name="product"/>. К списку заказов будет добавлена партия <paramref name="lot"/> 
    /// (если она не равна null).
    /// </summary>
    /// <param name="contractor"></param>
    /// <param name="product"></param>
    /// <param name="lot"></param>
    /// <returns></returns>
    IReadOnlyList<ProductionLot> GetActive(Contractor contractor, Goods product, ProductionLot? lot);

    /// <summary>
    /// Функция возвращает список партий находящихся в работе либо выполненных. Остаток изделий созданных в этой партии
    /// находящийся на складе не должен быть равен 0. Партия создана в рамках заказа контрагента <paramref name="contractor"/> и 
    /// изготовления <paramref name="product"/>. К списку заказов будет добавлена партия <paramref name="lot"/> 
    /// (если она не равна null).
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="contractor"></param>
    /// <param name="product"></param>
    /// <param name="lot"></param>
    /// <returns></returns>
    IReadOnlyList<ProductionLot> GetActive(IDbConnection connection, Contractor contractor, Goods product, ProductionLot? lot);

    /// <summary>
    /// Функция возвращает список партий находящихся в работе либо выполненных. Остаток изделий созданных в этой партии
    /// находящийся на складе не должен быть равен 0. Партия создана в рамках заказа для изготовления <paramref name="product"/>. 
    /// К списку заказов будет добавлена партия <paramref name="lot"/> (если она не равна null).
    /// </summary>
    /// <param name="product"></param>
    /// <param name="lot"></param>
    /// <returns></returns>
    IReadOnlyList<ProductionLot> GetActive(Goods product, ProductionLot? lot);

    /// <summary>
    /// Функция возвращает список партий находящихся в работе либо выполненных. Остаток изделий созданных в этой партии
    /// находящийся на складе не должен быть равен 0. Партия создана в рамках заказа для изготовления <paramref name="product"/>.
    /// К списку заказов будет добавлена партия <paramref name="lot"/> (если она не равна null).
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="product"></param>
    /// <param name="lot"></param>
    /// <returns></returns>
    IReadOnlyList<ProductionLot> GetActive(IDbConnection connection, Goods product, ProductionLot? lot);

    /// <summary>
    /// Функция возвращает список партий находящихся в работе либо выполненных. Остаток изделий созданных в этой партии
    /// находящийся на складе не должен быть равен 0. Партия создана в рамках заказа контрагента <paramref name="contractor"/>.
    /// К списку заказов будет добавлена партия <paramref name="lot"/> (если она не равна null).
    /// </summary>
    /// <param name="contractor"></param>
    /// <param name="lot"></param>
    /// <returns></returns>
    IReadOnlyList<ProductionLot> GetActive(Contractor contractor, ProductionLot? lot);

    /// <summary>
    /// Функция возвращает список партий находящихся в работе либо выполненных. Остаток изделий созданных в этой партии
    /// находящийся на складе не должен быть равен 0. Партия создана в рамках заказа контрагента <paramref name="contractor"/>.
    /// К списку заказов будет добавлена партия <paramref name="lot"/> (если она не равна null).
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="contractor"></param>
    /// <param name="lot"></param>
    /// <returns></returns>
    IReadOnlyList<ProductionLot> GetActive(IDbConnection connection, Contractor contractor, ProductionLot? lot);

    /// <summary>
    /// Функция возвращает количество изделий изготовленные в партии <paramref name="lot"/> отсутствующие в 
    /// списке готовых изделий.
    /// </summary>
    /// <param name="lot">Партия для которой производится подсчет изготовленных изделлий.</param>
    /// <returns></returns>
    decimal GetNewlyManufacturedProducts(ProductionLot lot);

    /// <summary>
    /// Функция возвращает количество изделий изготовленные в партии <paramref name="lot"/> отсутствующие в 
    /// списке готовых изделий.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="lot">Партия для которой производится подсчет изготовленных изделлий.</param>
    /// <returns></returns>
    decimal GetNewlyManufacturedProducts(IDbConnection connection, ProductionLot lot);
}
