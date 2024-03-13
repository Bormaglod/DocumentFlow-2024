//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;


public interface IOperationRepository : IBaseOperationRepository<Operation>
{
    /// <summary>
    /// Функция возвращает список изделий в которых указанная операция иожет быть использована.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="operation"></param>
    /// <returns></returns>
    IList<OperationGoods> GetGoods(Operation operation);

    /// <summary>
    /// Функция возвращает список изделий в которых указанная операция иожет быть использована.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="operation"></param>
    /// <returns></returns>
    IList<OperationGoods> GetGoods(IDbConnection connection, Operation operation);
}
