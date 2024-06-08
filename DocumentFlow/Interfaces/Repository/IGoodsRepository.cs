//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IGoodsRepository : IProductRepository<Goods>
{
    /// <summary>
    /// Функция возвращает список утверждённых калькуляций для указанного изделия. Также будет добавлена калькуляция указанная либо
    /// в параметре <paramref name="calculation"/>, либо в свойстве <see cref="Goods.Calculation"/>. Приоритет в данном случае у параметра,
    /// т.е. если он установлен, то именно он будет использован.
    /// </summary>
    /// <param name="goods"></param>
    /// <param name="calculation"></param>
    /// <returns></returns>
    IReadOnlyList<Calculation> GetCalculations(Goods goods, Calculation? calculation = null);

    /// <summary>
    /// Функция возвращает список утверждённых калькуляций для указанного изделия. Также будет добавлена калькуляция указанная либо
    /// в параметре <paramref name="calculation"/>, либо в свойстве <see cref="Goods.Calculation"/>. Приоритет в данном случае у параметра,
    /// т.е. если он установлен, то именно он будет использован.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="goods"></param>
    /// <param name="calculation"></param>
    /// <returns></returns>
    IReadOnlyList<Calculation> GetCalculations(IDbConnection connection, Goods goods, Calculation? calculation = null);

    /// <summary>
    /// Функция возвращает текущую калькуляцию для изделия <paramref name="goods"/> или null, если она не установлена.
    /// </summary>
    /// <param name="goods"></param>
    /// <returns></returns>
    Calculation? GetCurrentCalculation(Goods goods);

    /// <summary>
    /// Функция возвращает текущую калькуляцию для изделия <paramref name="goods"/> или null, если она не установлена.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="goods"></param>
    /// <returns></returns>
    Calculation? GetCurrentCalculation(IDbConnection connection, Goods goods);

    void CopyCalculation(Goods fromProduct, Goods toProduct);

    void CopyCalculation(IDbConnection connection, Goods fromProduct, Goods toProduct, IDbTransaction? transaction);
}