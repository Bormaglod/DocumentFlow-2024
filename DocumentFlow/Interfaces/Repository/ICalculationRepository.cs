//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface ICalculationRepository : IDirectoryRepository<Calculation>
{
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

    void CopyItems(Calculation fromCalculation, Calculation toCalculation);

    void CopyItems(IDbConnection connection, Calculation fromCalculation, Calculation toCalculation, IDbTransaction? transaction = null);

    /// <summary>
    /// Возвращает список операций относящихся к калькуляции <paramref name="calculation"/>.
    /// Если параметр <paramref name="includeMaterialInfo"/> равен true, то для каждого значения <see cref="CalculationOperation"/>
    /// будет установлено свойство <see cref="CalculationOperation.Material"/>.
    /// </summary>
    /// <param name="calculation"></param>
    /// <param name="includeMaterialInfo"></param>
    /// <returns></returns>
    IReadOnlyList<CalculationOperation> GetOperations(Calculation calculation, bool includeMaterialInfo = false);

    /// <summary>
    /// Возвращает список операций относящихся к калькуляции <paramref name="calculation"/>.
    /// Если параметр <paramref name="includeMaterialInfo"/> равен true, то для каждого значения <see cref="CalculationOperation"/>
    /// будет установлено свойство <see cref="CalculationOperation.Material"/>.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="calculation"></param>
    /// <param name="includeMaterialInfo"></param>
    /// <returns></returns>
    IReadOnlyList<CalculationOperation> GetOperations(IDbConnection connection, Calculation calculation, bool includeMaterialInfo = false);
}