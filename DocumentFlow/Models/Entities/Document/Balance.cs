//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class Balance : BaseDocument
{
    /// <summary>
    /// Возвращает или устанавливает идентификатор записи являющийся ссылкой на справочник по которому считаются остатки.
    /// </summary>
    [ObservableProperty]
    private Guid referenceId;

    /// <summary>
    /// Возвращает или устанавливает сумму операции.
    /// </summary>
    [ObservableProperty]
    private decimal operationSumma;

    /// <summary>
    /// Возвращает или устанавливает количественный оборот.
    /// </summary>
    [ObservableProperty]
    private decimal amount;

    /// <summary>
    /// Возвращает код документа который сформировал эту запись.
    /// </summary>
    public string DocumentCode { get; protected set; } = string.Empty;

    /// <summary>
    /// Возвращает наименование документа который сформировал эту запись.
    /// </summary>
    public string DocumentName { get; protected set; } = string.Empty;
}
