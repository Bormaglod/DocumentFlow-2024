//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface ICalculationOperationRepository : IBaseCalculationOperationRepository<CalculationOperation>
{
    /// <summary>
    /// Функция возвращает список устаноленных свойств для указанного материала.
    /// </summary>
    /// <param name="material"></param>
    /// <returns></returns>
    IList<CalculationOperationProperty> GetProperties(CalculationOperation operation);

    /// <summary>
    /// Функция возвращает список устаноленных свойств для указанного материала.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="material"></param>
    /// <returns></returns>
    IList<CalculationOperationProperty> GetProperties(IDbConnection connection, CalculationOperation operation);

    /// <summary>
    /// Функция возвращает список операций, которые могут предшествовать указанной.
    /// </summary>
    /// <param name="calculation"></param>
    /// <param name="operation"></param>
    /// <returns></returns>
    IReadOnlyList<CalculationOperation> GetPreviousOperations(Calculation calculation, CalculationOperation? operation);

    /// <summary>
    /// Функция возвращает список операций, которые могут предшествовать указанной.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="calculation"></param>
    /// <param name="operation"></param>
    /// <returns></returns>
    IReadOnlyList<CalculationOperation> GetPreviousOperations(IDbConnection connection, Calculation calculation, CalculationOperation? operation);
}