//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Common.Exceptions;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata.Execution;
using System.Data;

namespace DocumentFlow.Repository;

public abstract class CalculationItemRepository<T>(IDatabase database) : DirectoryRepository<T>(database), ICalculationItemRepository<T>
    where T : CalculationItem
{
    public decimal GetSumItemCost(Calculation calculation)
    {
        using var conn = GetConnection();
        return GetSumItemCost(conn, calculation);
    }

    public decimal GetSumItemCost(IDbConnection connection, Calculation calculation)
    {
        return connection.GetQuery<T>()
            .AsSum("item_cost")
            .Where("owner_id", calculation.Id)
            .Get<decimal>()
            .SingleOrDefault();
    }

    public void RecalculatePrices(Calculation calculation)
    {
        using var conn = GetConnection();
        using var trans = conn.BeginTransaction();
        RecalculatePrices(conn, calculation, trans);
    }

    public void RecalculatePrices(IDbConnection connection, Calculation calculation, IDbTransaction? transaction = null)
    {
        ExecuteProcedure(connection, "make_prices_operations_relevant", calculation, transaction);
    }

    protected void ExecuteProcedure(IDbConnection connection, string proc_name, Calculation calculation, IDbTransaction? transaction = null)
    {
        try
        {
            connection.Execute($"call {proc_name}(:Id)", calculation, transaction);
            transaction?.Commit();
        }
        catch (Exception e)
        {
            transaction?.Rollback();
            throw new RepositoryException(ExceptionHelper.Message(e), e);
        }
    }

    protected void ExecuteSql(IDbConnection connection, string sql, CalculationItem item, IDbTransaction? transaction = null)
    {
        try
        {
            connection.Execute(sql, item, transaction);
            transaction?.Commit();
        }
        catch (Exception e)
        {
            transaction?.Rollback();
            throw new RepositoryException(ExceptionHelper.Message(e), e);
        }
    }
}
