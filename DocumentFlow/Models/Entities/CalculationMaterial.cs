//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models.Entities;

public class CalculationMaterial : CalculationItem
{
    /*private decimal amount;
    private bool isGiving;
    private string priceSettingMethod = "average";

    /// <summary>
    /// Возвращает или устанавливает расход материала на изделие.
    /// </summary>
    public decimal Amount 
    { 
        get => amount;
        set => SetProperty(ref amount, value);
    }

    /// <summary>
    /// Возвращает или устанавливает флаг определяющий - является ли материал давальческим.
    /// </summary>
    public bool IsGiving 
    { 
        get => isGiving;
        set => SetProperty(ref isGiving, value);
    }

    [EnumType("price_setting_method")]
    public string PriceSettingMethod 
    { 
        get => priceSettingMethod;
        set => SetProperty(ref priceSettingMethod, value);
    }

    [Computed]
    public string? MaterialName { get; protected set; }

    [Computed]
    public decimal Weight { get; protected set; }

    [Write(false)]
    public PriceSettingMethod MethodPrice
    {
        get { return Enum.Parse<PriceSettingMethod>(PriceSettingMethod.Pascalize()); }
        set { PriceSettingMethod = value.ToString().Underscore(); }
    }

    public string MethodPriceName => MethodPrice.Description();

    public override string ToString() => MaterialName ?? "[NULL]";*/
}
