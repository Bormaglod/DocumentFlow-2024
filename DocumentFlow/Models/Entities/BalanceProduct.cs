//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models.Entities;

public class BalanceProduct : Balance
{
    [Display(Name = "Приход", Order = 500)]
    public decimal? Income { get; protected set; }

    [Display(Name = "Расход", Order = 600)]
    public decimal? Expense { get; protected set; }

    [Display(Name = "Остаток", Order = 700)]
    public decimal Remainder { get; protected set; }

    [Display(Name = "Остаток, руб.", Order = 800)]
    public decimal MonetaryBalance { get; protected set; }
}
