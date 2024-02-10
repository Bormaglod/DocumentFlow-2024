//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Models.Entities;

public partial class Goods : Product
{
    /// <summary>
    /// Возвращает или устанавливает флаг означающий, что данный продукт является услугой (или нет).
    /// </summary>
    [ObservableProperty]
    private bool isService;

    /// <summary>
    /// Возвращает или устанавливает идентификатор калькуляции.
    /// </summary>
    [ObservableProperty]
    private Calculation? calculation;

    /// <summary>
    /// Возвращает или устанавливает дополнительную информацию об изделии/услуге.
    /// </summary>
    [ObservableProperty]
    private string? note;

    /// <summary>
    /// Возвращает или устанавливает длину изделия.
    /// </summary>
    [ObservableProperty]
    private int? length;

    /// <summary>
    /// Возвращает или устанавливает ширину изделия.
    /// </summary>
    [ObservableProperty]
    private int? width;

    /// <summary>
    /// Возвращает или устанавливает высоту изделия.
    /// </summary>
    [ObservableProperty]
    private int? height;
}
