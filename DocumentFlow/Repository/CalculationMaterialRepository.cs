//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Repository;

public class CalculationMaterialRepository : CalculationItemRepository<CalculationMaterial>, ICalculationMaterialRepository, ITransientLifetime
{
    public CalculationMaterialRepository(IDatabase database) : base(database) { }

    public void RecalculateCount(Calculation calculation)
    {
        using var conn = GetConnection();
        using var trans = conn.BeginTransaction();
        RecalculateCount(conn, calculation, trans);
    }

    public void RecalculateCount(IDbConnection connection, Calculation calculation, IDbTransaction? transaction = null)
    {
        ExecuteProcedure(connection, "recalculate_amount_material", calculation, transaction);
    }

    public void SetPriceSettingMethod(CalculationMaterial material)
    {
        using var conn = GetConnection();
        using var trans = conn.BeginTransaction();
        SetPriceSettingMethod(conn, material, trans);
    }

    public void SetPriceSettingMethod(IDbConnection connection, CalculationMaterial material, IDbTransaction? transaction = null)
    {
        ExecuteSql(connection, "update calculation_material set price_method = :PriceMethod::price_setting_method where id = :Id", material, transaction);
    }

    public void SetPrice(CalculationMaterial material)
    {
        using var conn = GetConnection();
        using var trans = conn.BeginTransaction();
        SetPrice(conn, material, trans);
    }

    public void SetPrice(IDbConnection connection, CalculationMaterial material, IDbTransaction? transaction = null)
    {
        ExecuteSql(connection, "update calculation_material set price = :Price where id = :Id", material, transaction);
    }

    public void RefreshPrices(CalculationMaterial material)
    {
        using var conn = GetConnection();
        RefreshPrices(conn, material);
    }

    public void RefreshPrices(IDbConnection connection, CalculationMaterial material)
    {
        var sql = "select price, item_cost from calculation_material where id = :Id";
        var res = connection.Query<CalculationMaterial>(sql, material).First();
        material.Price = res.Price;
        material.ItemCost = res.ItemCost;
    }
}