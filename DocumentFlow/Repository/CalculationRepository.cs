﻿//-----------------------------------------------------------------------
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

using Humanizer;

using System.Data;

namespace DocumentFlow.Repository;

public class CalculationRepository : DirectoryRepository<Calculation>, ICalculationRepository, ITransientLifetime
{
    public CalculationRepository(IDatabase database) : base(database) { }

    public void CopyItems(Calculation fromCalculation, Calculation toCalculation)
    {
        using var conn = GetConnection();
        using var transaction = conn.BeginTransaction();

        try
        {
            CopyItems(conn, fromCalculation, toCalculation, transaction);
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new RepositoryException(ExceptionHelper.Message(e));
        }
    }

    public void CopyItems(IDbConnection connection, Calculation fromCalculation, Calculation toCalculation, IDbTransaction? transaction = null)
    {
        var sql = "insert into calculation_operation (owner_id, code, item_name, item_id, equipment_id, tools_id, material_id, material_amount, repeats, previous_operation, note) select :id_to, code, item_name, item_id, equipment_id, tools_id, material_id, material_amount, repeats, previous_operation, note from only calculation_operation where owner_id = :id_from";
        connection.Execute(sql, new { id_to = toCalculation.Id, id_from = fromCalculation.Id }, transaction: transaction);

        sql = "insert into calculation_cutting (owner_id, code, item_name, item_id, equipment_id, tools_id, material_id, material_amount, repeats, previous_operation, note) select :id_to, code, item_name, item_id, equipment_id, tools_id, material_id, material_amount, repeats, previous_operation, note from calculation_cutting where owner_id = :id_from";
        connection.Execute(sql, new { id_to = toCalculation.Id, id_from = fromCalculation.Id }, transaction: transaction);

        sql = "insert into calculation_deduction (owner_id, code, item_name, item_id, price, item_cost, value) select :id_to, code, item_name, item_id, price, item_cost, value from calculation_deduction where owner_id = :id_from";
        connection.Execute(sql, new { id_to = toCalculation.Id, id_from = fromCalculation.Id }, transaction: transaction);
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