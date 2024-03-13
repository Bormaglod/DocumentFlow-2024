//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.Interfaces.Repository;

public interface ICalculationMaterialRepository : ICalculationItemRepository<CalculationMaterial>
{
    void RecalculateCount(Calculation calculation);
    void RecalculateCount(IDbConnection connection, Calculation calculation, IDbTransaction? transaction = null);
    void SetPriceSettingMethod(CalculationMaterial material);
    void SetPriceSettingMethod(IDbConnection connection, CalculationMaterial material, IDbTransaction? transaction = null);
    void SetPrice(CalculationMaterial material);
    void SetPrice(IDbConnection connection, CalculationMaterial material, IDbTransaction? transaction = null);
    void RefreshPrices(CalculationMaterial material);
    void RefreshPrices(IDbConnection connection, CalculationMaterial material);

}
