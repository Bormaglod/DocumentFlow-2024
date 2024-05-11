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


public partial class Calculation : Directory
{
    [ObservableProperty]
    [property: DenyWriting]
    [property: ForeignKey(FieldKey = "owner_id")]
    private Goods? goods;

    /// <summary>
    /// Возвращает или устанавливает себестоимость изделия или услуги.
    /// </summary>
    [ObservableProperty]
    private decimal costPrice;

    /// <summary>
    /// Возвращает или устанавливает рентабельность при изготовлении изделия или оказания услуги.
    /// </summary>
    [ObservableProperty]
    private decimal profitPercent;

    /// <summary>
    /// Возвращает или устанавливает норму прибыли при изготовлении изделия или оказания услуги.
    /// </summary>
    [ObservableProperty]
    private decimal profitValue;

    /// <summary>
    /// Возвращает или устанавливает цену изделия или услуги без учёта НДС.
    /// </summary>
    [ObservableProperty]
    private decimal price;

    /// <summary>
    /// Возвращает или устанавливает дополнительную информацию.
    /// </summary>
    [ObservableProperty]
    private string? note;

    /// <summary>
    /// Возвращает или устанавливает размер стимулирующей выплаты.
    /// </summary>
    [ObservableProperty]
    private decimal stimulPayment;

    /// <summary>
    /// Возвращает или устанавливает дату утверждения калькуляции.
    /// </summary>
    [ObservableProperty]
    private DateTime? dateApproval;

    /// <summary>
    /// Возвращает или устанавливает состояние калькуляции.
    /// </summary>
    [EnumType("calculation_state")]
    public string State { get; set; } = "prepare";

    /// <summary>
    /// Возвращает или устанавливает способ начисления стимулирующих выплат.
    /// </summary>
    [EnumType("stimulating_value")]
    public string StimulType { get; set; } = "money";

    /// <summary>
    /// Возвращает вес изделия изготовленного согласно данной калькуляции.
    /// </summary>
    public decimal Weight { get; protected set; }

    /// <summary>
    /// Возвращает время изготовления изделия в секундах.
    /// </summary>
    public decimal ProducedTime { get; protected set; }

    [DenyWriting]
    public CalculationState CalculationState
    {
        get { return Enum.Parse<CalculationState>(State.Pascalize()); }
        set { State = value.ToString().Underscore(); }
    }

    [DenyWriting]
    public StimulatingValue StimulatingValue
    {
        get { return Enum.Parse<StimulatingValue>(StimulType.Pascalize()); }
        set { StimulType = value.ToString().Underscore(); }
    }

    public override string ToString()
    {
        if (DateApproval == null)
        {
            return Code;
        }

        return $"{Code} от {DateApproval:d}";
    }
}
