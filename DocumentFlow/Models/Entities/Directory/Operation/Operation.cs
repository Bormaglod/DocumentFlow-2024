//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class Operation : Directory
{
    /// <summary>
    /// Возвращает или устанавливает выработку за указанное время <see cref="ProdTime"/>.
    /// </summary>
    [ObservableProperty]
    private int produced;

    /// <summary>
    /// Возвращает или устанавливает время за которое было произведено указанное количество операций <see cref="Produced"/> (указывается в минутах).
    /// </summary>
    [ObservableProperty]
    private int prodTime;

    /// <summary>
    /// Возвращает или устанавливает норму выработки (шт./час).
    /// </summary>
    [ObservableProperty]
    private int productionRate;

    //private Guid typeId;

    /// <summary>
    /// Возвращает или устанавливает дату нормирования.
    /// </summary>
    [ObservableProperty]
    private DateTime? dateNorm;

    /// <summary>
    /// Возвращает или устанавливает плату за выполнение ед. операции
    /// </summary>
    [ObservableProperty]
    private decimal salary;

    /// <summary>
    /// Возвращает или устанавливает тип операции <seealso cref="OperationTypes.OperationType"/>.
    /// </summary>
    [ObservableProperty]
    [property: ForeignKey(FieldKey = "type_id")]
    private OperationType? operationType;

    public bool OperationUsing { get; protected set; }
}
