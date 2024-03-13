//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class CalculationOperation : CalculationItem
{
    /// <summary>
    /// Возвращает или устанавливает производственную операцию для данного пункта калькуляции.
    /// </summary>
    [ObservableProperty]
    [property: ForeignKey(FieldKey = "item_id")]
    private Operation? operation;

    /// <summary>
    /// Возвращает или устанавливает оборудование на котором выполняется операция.
    /// </summary>
    [ObservableProperty]
    private Equipment? equipment;

    /// <summary>
    /// Возвращает или устанавливает инструмент с помощью которого выполняется операция.
    /// </summary>
    [ObservableProperty]
    [property: ForeignKey(FieldKey = "tools_id")]
    private Equipment? tool;

    /// <summary>
    /// Возвращает или устанавливает материал используемый при выполнении операции.
    /// </summary>
    [ObservableProperty]
    private Material? material;

    /// <summary>
    /// Возвращает или устанавливает количество используемого материала на 1 опер. (в ед. изм. этой операции)
    /// </summary>
    [ObservableProperty]
    private decimal materialAmount;

    /// <summary>
    /// Возвращает или устанавливает количество повторов операции.
    /// </summary>
    [ObservableProperty]
    private int repeats = 1;

    /// <summary>
    /// Возвращает или устанавливает список кодов операций результат которых используется в текущей
    /// </summary>
    [ObservableProperty]
    private string[]? previousOperation;

    /// <summary>
    /// Возвращает или устанавливает дополнительную информацию об операции.
    /// </summary>
    [ObservableProperty]
    private string? note;

    /// <summary>
    /// Возвращает или устанавливает стимулирующую выплату для данной операции.
    /// </summary>
    [ObservableProperty]
    private decimal stimulCost;

    /// <summary>
    /// Возвращает или устанавливает графическую информацию об операции.
    /// </summary>
    [ObservableProperty]
    private byte[]? preview;

    /// <summary>
    /// Возвращает список кодов операций в которых используется данная операция.
    /// </summary>
    public string[]? UsingOperations { get; protected set; }

    public decimal TotalMaterial { get; protected set; }

    public decimal ProducedTime { get; protected set; }
}
