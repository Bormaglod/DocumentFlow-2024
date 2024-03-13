//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface ICalculationItemRepository<T> : IDirectoryRepository<T>
    where T : CalculationItem
{
    decimal GetSumItemCost(Calculation calculation);
    decimal GetSumItemCost(IDbConnection connection, Calculation calculation);
    void RecalculatePrices(Calculation calculation);
    void RecalculatePrices(IDbConnection connection, Calculation calculation, IDbTransaction? transaction = null);
}
