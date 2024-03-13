//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Models.Entities;

public partial class BalanceContractor : Balance
{
    /// <summary>
    /// Возвращает или устанавливает договор для которого формируется данный остаток.
    /// </summary>
    [ObservableProperty]
    private Contract? contract;

    public decimal? ContractorDebt { get; protected set; }
    public decimal? OrganizationDebt { get; protected set; }
    public decimal Debt { get; protected set; }
}
