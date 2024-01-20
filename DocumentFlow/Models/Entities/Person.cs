//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Models.Entities;

public partial class Person : Directory
{
    [ObservableProperty]
    private string? surname;

    [ObservableProperty]
    private string? firstName;

    [ObservableProperty]
    private string? middleName;

    [ObservableProperty]
    private string? phone;

    [ObservableProperty]
    private string? email;
}
