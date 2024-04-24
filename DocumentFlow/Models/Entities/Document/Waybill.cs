//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Models.Entities;

public partial class Waybill : ShipmentDocument
{
    /// <summary>
    /// Возвращает или устанавливает номер накладной (1С).
    /// </summary>
    [ObservableProperty]
    private string? waybillNumber;

    /// <summary>
    /// Возвращает или устанавливает дату выдачи накладной (1С).
    /// </summary>
    [ObservableProperty]
    private DateTime? waybillDate = DateTime.Now;

    /// <summary>
    /// Возвращает или устанавливает номер счёт-фактуры (1С).
    /// </summary>
    [ObservableProperty]
    private string? invoiceNumber;

    /// <summary>
    /// Возвращает или устанавливает дату выдачи счёт-фактуры (1С).
    /// </summary>
    [ObservableProperty]
    private DateTime? invoiceDate = DateTime.Now;

    /// <summary>
    /// Возвращает или устанавливает флаг, определяющий является ли документ универсальным передаточным документом или нет.
    /// </summary>
    [ObservableProperty]
    private bool upd;

    /// <summary>
    /// Возвращает полную стоимость товаров по накладной без НДС.
    /// </summary>
    public decimal ProductCost { get; protected set; }

    /// <summary>
    /// Возвращает значение ставки НДС.
    /// </summary>
    public int Tax => (Contract?.TaxPayer ?? false) ? 20 : 0;

    /// <summary>
    /// Возвращает сумму НДС всех товаров по накладной.
    /// </summary>
    public decimal TaxValue { get; protected set; }

    /// <summary>
    /// Возвращает полную стоимость товаров по накладной включая НДС.
    /// </summary>
    public decimal FullCost { get; protected set; }

    /// <summary>
    /// Возвращает сумму уже уплаченную за товар в накладной.
    /// </summary>
    public decimal Paid { get; protected set; }
}
