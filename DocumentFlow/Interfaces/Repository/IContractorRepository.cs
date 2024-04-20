//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface IContractorRepository : ICompanyRepository<Contractor>
{
    /// <summary>
    /// Функция возвращает список расчётных счетов указанного контрагента.
    /// </summary>
    /// <param name="contractor"></param>
    /// <returns></returns>
    IReadOnlyList<Account> GetAccounts(Contractor contractor);

    /// <summary>
    /// Функция возвращает список расчётных счетов указанного контрагента.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="contractor"></param>
    /// <returns></returns>
    IReadOnlyList<Account> GetAccounts(IDbConnection connection, Contractor contractor);

    /// <summary>
    /// Функция возвращает список всех договоров с указанным контрагентом.
    /// </summary>
    /// <param name="contractor"></param>
    /// <returns></returns>
    IReadOnlyList<Contract> GetContracts(Contractor contractor);

    /// <summary>
    /// Функция возвращает список всех договоров с указанным контрагентом.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="contractor"></param>
    /// <returns></returns>
    IReadOnlyList<Contract> GetContracts(IDbConnection connection, Contractor contractor);
}
