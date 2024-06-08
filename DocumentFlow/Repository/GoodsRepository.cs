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

public class GoodsRepository(IDatabase database) : 
    ProductRepository<Goods>(database), 
    IGoodsRepository, 
    ITransientLifetime
{
    public void CopyCalculation(Goods fromProduct, Goods toProduct)
    {
        using var conn = GetConnection();
        using var transaction = conn.BeginTransaction();

        try
        {
            CopyCalculation(conn, fromProduct, toProduct, transaction);
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new RepositoryException(ExceptionHelper.Message(e));
        }
    }

    public void CopyCalculation(IDbConnection connection, Goods fromProduct, Goods toProduct, IDbTransaction? transaction)
    {
        if (fromProduct.Calculation != null)
        {
            var newCalc = new Calculation()
            {
                OwnerId = toProduct.Id,
                ItemName = fromProduct.Calculation.ItemName,
                CostPrice = fromProduct.Calculation.CostPrice,
                ProfitPercent = fromProduct.Calculation.ProfitPercent,
                ProfitValue = fromProduct.Calculation.ProfitValue,
                Price = fromProduct.Calculation.Price,
                Note = fromProduct.Calculation.Note,
                StimulPayment = fromProduct.Calculation.StimulPayment,
                StimulType = fromProduct.Calculation.StimulType
            };

            connection.Insert(newCalc, transaction);

            var repo = ServiceLocator.Context.GetService<ICalculationRepository>();
            repo.CopyItems(connection, fromProduct.Calculation, newCalc, transaction);
        }
    }

    public IReadOnlyList<Calculation> GetCalculations(Goods goods, Calculation? calculation)
    {
        using var conn = GetConnection();
        return GetCalculations(conn, goods, calculation);
    }

    public IReadOnlyList<Calculation> GetCalculations(IDbConnection connection, Goods goods, Calculation? calculation)
    {
        var calc = calculation ?? goods.Calculation;
        return GetCustomSlimQuery<Calculation>(connection, goods)
            .Select("state")
            .WhereRaw("state = 'approved'::calculation_state")
            .When(calc != null, w => w.OrWhere("id", calc!.Id))
            .Get<Calculation>()
            .ToList();
    }

    public Calculation? GetCurrentCalculation(Goods goods)
    {
        using var conn = GetConnection();
        return GetCurrentCalculation(conn, goods);
    }

    public Calculation? GetCurrentCalculation(IDbConnection connection, Goods goods)
    {
        return connection.QueryFirstOrDefault<Calculation>("select c.* from calculation c join goods g on g.calculation_id = c.id where g.id = :Id", new { goods.Id });
    }
}