//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IBaseOperationRepository<T> : IDirectoryRepository<T>
    where T : Operation
{
    /// <summary>
    /// Функция возвращает список всех производственных операций.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<T> GetOperations();

    /// <summary>
    /// Функция возвращает список всех производственных операций.
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    IReadOnlyList<T> GetOperations(IDbConnection connection);
}
