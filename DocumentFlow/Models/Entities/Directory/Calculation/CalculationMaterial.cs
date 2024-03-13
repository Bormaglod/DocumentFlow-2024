//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;

using Humanizer;

namespace DocumentFlow.Models.Entities;

public partial class CalculationMaterial : CalculationItem
{
    [ObservableProperty]
    [property: ForeignKey(FieldKey = "item_id")]
    private Material? material;

    /// <summary>
    /// Возвращает или устанавливает расход материала на изделие.
    /// </summary>
    [ObservableProperty]
    private decimal amount;

    [EnumType("price_setting_method")]
    public string PriceMethod { get; set; } = "average";

    [DenyWriting]
    public PriceSettingMethod PriceSettingMethod
    {
        get { return Enum.Parse<PriceSettingMethod>(PriceMethod.Pascalize()); }
        set { PriceMethod = value.ToString().Underscore(); }
    }

    public decimal Weight { get; protected set; }

    public override string ToString() => Material?.ToString() ?? "[NULL]";
}
