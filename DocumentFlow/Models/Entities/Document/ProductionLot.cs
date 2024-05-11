//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class ProductionLot : AccountingDocument
{
    [ObservableProperty]
    [property: DenyWriting]
    [property: ForeignKey(FieldKey = "owner_id")]
    private ProductionOrder? order;

    /// <summary>
    /// Возвращает или устанавливает идентификатор калькуляции используемой для изготовления партии.
    /// </summary>
    [ObservableProperty]
    private Calculation? calculation;

    /// <summary>
    /// Возвращает или устанавливает количество изделий в партии.
    /// </summary>
    [ObservableProperty]
    private decimal quantity;

    /// <summary>
    /// Возвращает или устанавливает флаг определяющий, что партия реализована (если партия реализована частично - то NULL).
    /// </summary>
    [ObservableProperty]
    [property: DenyWriting]
    private bool? sold;

    /// <summary>
    /// Возвращает процент выполнения данной партии.
    /// </summary>
    public int ExecutePercent { get; protected set; }

    /// <summary>
    /// Возвращает количество нереализванных изделий из данной партии.
    /// </summary>
    public decimal FreeQuantity { get; protected set; }
}
