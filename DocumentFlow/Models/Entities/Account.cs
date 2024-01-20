//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class Account : Directory
{
    [ObservableProperty]
    private decimal accountValue;

    [ObservableProperty]
    private Bank? bank;

    [ObservableProperty]
    [property: DenyWriting]
    private Contractor? company;
}
