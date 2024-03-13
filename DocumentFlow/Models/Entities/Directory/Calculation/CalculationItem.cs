//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Models.Entities;

public partial class CalculationItem : Directory
{
    /// <summary>
    /// Возвращает или устанавливает цену за 1 ед. элемента калькуляции
    /// </summary>
    [ObservableProperty]
    private decimal price;

    /// <summary>
    /// Возвращает или устанавливает стоимость элемента калькуляции
    /// </summary>
    [ObservableProperty]
    private decimal itemCost;
}
