//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Models.Entities;

public partial class ContractApplication : Directory
{
    [ObservableProperty]
    private DateTime documentDate;

    [ObservableProperty]
    private DateTime dateStart;

    [ObservableProperty]
    private DateTime? dateEnd;

    [ObservableProperty]
    private string? note;

    public override string ToString() => $"{ItemName} №{Code} от {DocumentDate:d}";
}
