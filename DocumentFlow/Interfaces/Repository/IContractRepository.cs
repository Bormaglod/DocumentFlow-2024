//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Data.Models;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IContractRepository : IDirectoryRepository<Contract>
{
    /// <summary>
    /// Функция возвращает список действующих на указанную дату приложений к выбранному договору. Если дата не указана,
    /// то она считается текущей.
    /// </summary>
    /// <param name="contract"></param>
    /// <returns></returns>
    IReadOnlyList<ContractApplication> GetCurrentAnnexes(Contract contract, DateTime? actualDate = null);

    /// <summary>
    /// Функция возвращает список действующих на указанную дату приложений к выбранному договору. Если дата не указана,
    /// то она считается текущей.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="contract"></param>
    /// <param name="actualDate"></param>
    /// <returns></returns>
    IReadOnlyList<ContractApplication> GetCurrentAnnexes(IDbConnection connection, Contract contract, DateTime? actualDate = null);

    /// <summary>
    /// Функция возвращает цену указанного материала или изделия <paramref name="product"/> в выбранном договоре <paramref name="contract"/> на дату <paramref name="actualDate"/>.
    /// Если дата не указана, то она считается текущей.
    /// </summary>
    /// <param name="contract"></param>
    /// <param name="product"></param>
    /// <param name="actualDate"></param>
    /// <returns></returns>
    decimal GetPrice(Contract contract, Product product, DateTime? actualDate = null);

    /// <summary>
    /// Функция возвращает цену указанного материала или изделия <paramref name="product"/> в выбранном договоре <paramref name="contract"/> на дату <paramref name="actualDate"/>.
    /// Если дата не указана, то она считается текущей.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="contract"></param>
    /// <param name="product"></param>
    /// <param name="actualDate"></param>
    /// <returns></returns>
    decimal GetPrice(IDbConnection connection, Contract contract, Product product, DateTime? actualDate = null);
}
