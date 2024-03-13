//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface ICuttingRepository : IBaseOperationRepository<Cutting>
{
    /// <summary>
    /// Функция возвращает список доступных для установки программ для указанной операции.
    /// </summary>
    /// <param name="cutting"></param>
    /// <returns></returns>
    IReadOnlyList<int> GetAvailableProgram(Cutting? cutting);

    /// <summary>
    /// Функция возвращает список доступных для установки программ для указанной операции.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="cutting"></param>
    /// <returns></returns>
    IReadOnlyList<int> GetAvailableProgram(IDbConnection connection, Cutting? cutting);
}