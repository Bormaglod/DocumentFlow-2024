//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Editors;

public partial class CuttingViewModel : BaseOperationViewModel<Cutting>, ISelfTransientLifetime
{
    [ObservableProperty]
    private int segmentLength;

    [ObservableProperty]
    private decimal leftCleaning;

    [ObservableProperty]
    private int leftSweep;

    [ObservableProperty]
    private decimal rightCleaning;

    [ObservableProperty]
    private int rightSweep;

    [ObservableProperty]
    private int? programNumber;

    protected override string GetStandardHeader() => "Резка";
}
