//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Common.Exceptions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public class CalculationRepository : DirectoryRepository<Calculation>, ICalculationRepository, ITransientLifetime
{
    public CalculationRepository(IDatabase database) : base(database) { }

    public IReadOnlyList<Calculation> GetCalculations(Goods goods)
    {
        using var conn = GetConnection();
        return GetCalculations(conn, goods);
    }

    public IReadOnlyList<Calculation> GetCalculations(IDbConnection connection, Goods goods)
    {
        return GetSlimQuery(connection, goods)
            .WhereRaw("state = 'approved'::calculation_state")
            .When(goods.Calculation != null, w => w.OrWhere("id", goods.Calculation!.Id))
            .Get<Calculation>()
            .ToList();
    }

    public void SetState(Calculation calculation)
    {
        using var conn = GetConnection();
        using var trans = conn.BeginTransaction();
        SetState(conn, calculation, trans);
    }

    public void SetState(IDbConnection connection, Calculation calculation, IDbTransaction? transaction = null)
    {
        try
        {
            connection.Execute("update calculation set state = :State::calculation_state where id = :Id", calculation, transaction);
            transaction?.Commit();
        }
        catch (Exception e)
        {
            transaction?.Rollback();
            throw new RepositoryException(ExceptionHelper.Message(e), e);
        }
    }
}