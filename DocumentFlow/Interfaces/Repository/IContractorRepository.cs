//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;
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
    /// Функция возвращает список всех договоров с указанным контрагентом <paramref name="contractor"/> и его типом <paramref name="contractorType"/>.
    /// </summary>
    /// <param name="contractor"></param>
    /// <param name="contractorType"></param>
    /// <returns></returns>
    IReadOnlyList<Contract> GetContracts(Contractor contractor, ContractorType? contractorType = null);

    /// <summary>
    /// Функция возвращает список всех договоров с указанным контрагентом <paramref name="contractor"/> и его типом <paramref name="contractorType"/>.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="contractor"></param>
    /// <param name="contractorType"></param>
    /// <returns></returns>
    IReadOnlyList<Contract> GetContracts(IDbConnection connection, Contractor contractor, ContractorType? contractorType = null);

    /// <summary>
    /// Функция возвращает список поставщиков.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<Contractor> GetSuppliers();

    /// <summary>
    /// Функция возвращает список поставщиков.
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    IReadOnlyList<Contractor> GetSuppliers(IDbConnection connection);

    /// <summary>
    /// Функция возвращает список покупателей (клиентов).
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<Contractor> GetCustomers();

    /// <summary>
    /// Функция возвращает список покупателей (клиентов).
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    IReadOnlyList<Contractor> GetCustomers(IDbConnection connection);
}
