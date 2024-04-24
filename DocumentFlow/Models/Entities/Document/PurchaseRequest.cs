//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Models.Entities;

public partial class PurchaseRequest : ShipmentDocument
{
    [ObservableProperty]
    private string? note;

    /// <summary>
    /// Возвращает сумму заявки без НДС.
    /// </summary>
    public decimal CostOrder { get; protected set; }

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

    /// <summary>
    /// Возвращает сумму предоплаты.
    /// </summary>
    public decimal Prepayment { get; protected set; }

    /// <summary>
    /// Возвращает сумму полученного материала.
    /// </summary>
    public decimal ReceiptPayment { get; protected set; }

    /// <summary>
    /// Возвращает количество полученного материала.
    /// </summary>
    public decimal DeliveryAmount { get; protected set; }

    /// <summary>
    /// Возвращает true, если заявка выполнена полностью (или частично).
    /// </summary>
    public bool Executed { get => DeliveryAmount > 0; }

    /// <summary>
    /// Возвращает true, если заявка оплачена полностью, null - если есть переплата или требуется доплата
    /// </summary>
    public bool? Paid
    {
        get
        {
            if (Prepayment + ReceiptPayment == 0)
            {
                return false;
            }

            if (Prepayment + ReceiptPayment == DeliveryAmount)
            {
                return true;
            }

            return null;
        }
    }
}
