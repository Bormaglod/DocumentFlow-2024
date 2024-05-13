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
    /// Функция возвращает список утверждённых калькуляций для указанного изделия.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="goods"></param>
    /// <returns></returns>
    IReadOnlyList<Calculation> GetCalculations(Goods goods);

    /// <summary>
    /// Функция возвращает список утверждённых калькуляций для указанного изделия.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="goods"></param>
    /// <returns></returns>
    IReadOnlyList<Calculation> GetCalculations(IDbConnection connection, Goods goods);

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