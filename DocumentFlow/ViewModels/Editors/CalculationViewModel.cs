//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Editors;

public partial class CalculationViewModel : DirectoryEditorViewModel<Calculation>, ISelfTransientLifetime
{
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
    }

    partial void OnCodeChanged(string value)
    {
        UpdateHeader(value);
    }
}
