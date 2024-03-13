//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Date: 01.01.2022
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Models.Entities;

public partial class OperationType : Directory
{
    /// <summary>
    /// Возвращает или устанавливает базовую часовую ставку для расчёта заработной платы.
    /// </summary>
    [ObservableProperty]
    private decimal salary;
}
