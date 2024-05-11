//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Models.Entities;

namespace DocumentFlow.Common;

public partial class EmpInfo : BaseEmpInfo
{
    [ObservableProperty]
    private Employee employee;

    public EmpInfo(Employee employee)
    {
        Employee = employee;
    }

    public override string ToString() => Employee.ItemName ?? string.Empty;
}

