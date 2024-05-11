//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models.Entities;

public class ProductionOrder : ShipmentDocument
{
    /// <summary>
    /// Возвращает сумму заявки без НДС.
    /// </summary>
    public decimal ProductCost { get; protected set; }

    /// <summary>
    /// Возвращает значение ставки НДС.
    /// </summary>
    public int Tax => (Contract?.TaxPayer ?? false) ? 20 : 0;

    /// <summary>
    /// Возвращает сумму НДС.
    /// </summary>
    public decimal TaxValue { get; protected set; }

    /// <summary>
    /// Возвращает Полную стоимость заявки с НДС.
    /// </summary>
    public decimal FullCost { get; protected set; }
}