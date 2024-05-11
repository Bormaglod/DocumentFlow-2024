//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Common;

public abstract partial class BaseEmpInfo : ObservableObject
{
    [ObservableProperty]
    private long quantity;

    [ObservableProperty]
    private decimal salary;
}

