//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Models.Entities;

public partial class Bank : Directory
{
    [ObservableProperty]
    private decimal bik;

    [ObservableProperty]
    private decimal account;

    [ObservableProperty]
    private string? town;
}
