//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class OperationsPerformed : AccountingDocument
{
    /// <summary>
    /// Возвращает или устанавливает идентификатор сотрудника выполняющего операцию.
    /// </summary>
    [ObservableProperty]
    [ForeignKey(Table = "our_employee")]
    private Employee? employee;

    /// <summary>
    /// Возвращает или устанавливает идентификатор выполняемой сотрудником операции.
    /// </summary>
    [ObservableProperty]
    [property: ForeignKey(FieldKey = "operation_id")]
    private CalculationOperation? operation;

    /// <summary>
    /// Возвращает или устанавливает идентификатор фактически использованного материал для операции.
    /// </summary>
    [ObservableProperty]
    [property: ForeignKey(FieldKey = "replacing_material_id")]
    private Material? replacingMaterial;

    /// <summary>
    /// Возвращает или устанавливает количество выполненных операций.
    /// </summary>
    [ObservableProperty]
    private long quantity;

    /// <summary>
    /// Возвращает или устанавливает заработную плату, которая начислена за выполненную опрерацию.
    /// </summary>
    [ObservableProperty]
    private decimal salary;

    /// <summary>
    /// Возвращает или устанавливает флаг определяющий файк оплаты по двойному тарифу.
    /// </summary>
    [ObservableProperty]
    private bool? doubleRate;

    /// <summary>
    /// Возвращает или устанавливает флаг, который определяет возможность не учитывать используемый материал
    /// в операции.
    /// </summary>
    [ObservableProperty]
    private bool skipMaterial;

    [ObservableProperty]
    [property: DenyWriting]
    [property: ForeignKey(FieldKey = "owner_id")]
    private ProductionLot? lot;

    [ObservableProperty]
    [property: DenyWriting]
    [property: ForeignKey(FieldKey = "owner_id")]
    private Goods? goods;

    /// <summary>
    /// Возвращает значение материала использованного для выполнения данной операции. 
    /// Это либо материал заложенный в спецификацию <see cref="Operation"/> или его замена <see cref="ReplacingMaterial"/>. 
    /// </summary>
    public Material? UsingMaterial => ReplacingMaterial ?? Operation?.Material;
}