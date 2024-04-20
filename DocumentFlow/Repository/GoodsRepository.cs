//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common;
using DocumentFlow.Common.Exceptions;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public class GoodsRepository : ProductRepository<Goods>, IGoodsRepository, ITransientLifetime
{
    public GoodsRepository(IDatabase database) : base(database) { }

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

            connection.Update(newCalc, transaction);

            var repo = ServiceLocator.Context.GetService<ICalculationRepository>();
            repo.CopyItems(connection, fromProduct.Calculation, newCalc, transaction);
        }
    }

    public IReadOnlyList<Calculation> GetCalculations(Goods goods)
    {
        using var conn = GetConnection();
        return GetCalculations(conn, goods);
    }

    public IReadOnlyList<Calculation> GetCalculations(IDbConnection connection, Goods goods)
    {
        return GetCustomSlimQuery<Calculation>(connection, goods)
            .WhereRaw("state = 'approved'::calculation_state")
            .When(goods.Calculation != null, w => w.OrWhere("id", goods.Calculation!.Id))
            .Get<Calculation>()
            .ToList();
    }
}