//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Models.Entities;

public partial class Equipment : Directory
{
    /// <summary>
    /// Определяет является ли оборудование инструментом.
    /// </summary>
    [ObservableProperty]
    private bool isTools;

    /// <summary>
    /// Серийный номер оборудования.
    /// </summary>
    [ObservableProperty]
    private string? serialNumber;

    /// <summary>
    /// Дата ввода в эксплуатацию.
    /// </summary>
    [ObservableProperty]
    private DateTime? commissioning;

    /// <summary>
    /// Начальное количество опрессовок. Используется только для аппликаторов.
    /// </summary>
    [ObservableProperty]
    private int? startingHits;
}
