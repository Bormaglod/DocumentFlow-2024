//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.ComponentModel;

namespace DocumentFlow.ViewModels.Editors;

public partial class CalculationViewModel : DirectoryEditorViewModel<Calculation>, ISelfTransientLifetime, IDataErrorInfo
{
    private bool lockUpdate = false;

    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private decimal costPrice;

    [ObservableProperty]
    private decimal profitPercent;

    [ObservableProperty]
    private decimal profitValue;

    [ObservableProperty]
    private decimal price;

    [ObservableProperty]
    private string? note;

    [ObservableProperty]
    private DateTime? dateApproval;

    [ObservableProperty]
    private decimal stimulPayment;

    [ObservableProperty]
    private StimulatingValue stimulatingValue;

    [ObservableProperty]
    private CalculationState calculationState;

    public string Error => string.Empty;

    public string this[string name]
    {
        get
        {
            string result = null!;

            if (name == "Price")
            {
                if (Price < CostPrice)
                {
                    result = "Цена изделия должна быть больше себестоимости. Установка данного значения цены приводит к отрицательному значению рентабельности.";
                }
            }
            return result;
        }
    }

    protected override string GetStandardHeader() => "Калькуляция";

    protected override void RaiseAfterLoadDocument(Calculation entity)
    {
        Code = entity.Code;
        CostPrice = entity.CostPrice;
        ProfitPercent = entity.ProfitPercent;
        ProfitValue = entity.ProfitValue;
        Price = entity.Price;
        Note = entity.Note;
        DateApproval = entity.DateApproval;
        StimulatingValue = entity.StimulatingValue;
        StimulPayment = entity.StimulPayment;
        CalculationState = entity.CalculationState;
    }

    protected override void UpdateEntity(Calculation entity)
    {
        entity.Code = Code;
        entity.CostPrice = CostPrice;
        entity.ProfitPercent = ProfitPercent;
        entity.ProfitValue = ProfitValue;
        entity.Price = Price;
        entity.Note = Note;
        entity.DateApproval = DateApproval;
        entity.StimulatingValue = StimulatingValue;
        entity.StimulPayment = StimulPayment;
    }

    protected override void RegisterReports()
    {
        base.RegisterReports();
        RegisterReport(Report.Specification);
        RegisterReport(Report.ProcessMap);
    }

    partial void OnCodeChanged(string value)
    {
        UpdateHeader(value);
    }

    partial void OnProfitPercentChanged(decimal value)
    {
        if (Status != EntityEditStatus.Loaded || lockUpdate)
        {
            return;
        }

        try
        {
            lockUpdate = true;

            ProfitValue = CostPrice * value / 100;
            Price = CostPrice + ProfitValue;
        }
        finally
        {
            lockUpdate = false;
        }
        
    }


    partial void OnProfitValueChanged(decimal value)
    {
        if (Status != EntityEditStatus.Loaded || lockUpdate)
        {
            return;
        }

        try
        {
            lockUpdate = true;

            Price = CostPrice + value;
            ProfitPercent = value * 100 / CostPrice;
        }
        finally
        {
            lockUpdate = false;
        }
        
    }

    partial void OnPriceChanged(decimal value)
    {
        if (Status != EntityEditStatus.Loaded || lockUpdate)
        {
            return;
        }

        try
        {
            lockUpdate = true;

            ProfitValue = value - CostPrice;
            ProfitPercent = ProfitValue * 100 / CostPrice;
        }
        finally
        {
            lockUpdate = false;
        }
    }
}
