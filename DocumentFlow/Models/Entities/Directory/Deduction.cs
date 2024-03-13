//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;

using Humanizer;

using System.ComponentModel;

namespace DocumentFlow.Models.Entities;

[Description("Удержание")]
public partial class Deduction : Directory
{
    [ObservableProperty]
    private Person? person;

    [ObservableProperty]
    private decimal value;

    [EnumType("base_deduction")]
    public string BaseCalc { get; set; } = "material";

    public decimal? FixValue => BaseDeduction == BaseDeduction.Person ? Value : null;

    public decimal? PercentValue => BaseDeduction != BaseDeduction.Person ? Value : null;

    [DenyWriting]
    public BaseDeduction BaseDeduction
    {
        get => Enum.Parse<BaseDeduction>(BaseCalc.Pascalize());
        set => BaseCalc = value.ToString().Underscore();
    }
}
