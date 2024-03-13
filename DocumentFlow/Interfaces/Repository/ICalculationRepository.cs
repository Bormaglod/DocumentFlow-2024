//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface ICalculationRepository : IDirectoryRepository<Calculation>
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
    /// Метод сохраняет установленное состояние калькуляции.
    /// </summary>
    /// <param name="calculation"></param>
    void SetState(Calculation calculation);

    /// <summary>
    /// Метод сохраняет установленное состояние калькуляции.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="calculation"></param>
    /// <param name="transaction"></param>
    void SetState(IDbConnection connection, Calculation calculation, IDbTransaction? transaction = null);
}