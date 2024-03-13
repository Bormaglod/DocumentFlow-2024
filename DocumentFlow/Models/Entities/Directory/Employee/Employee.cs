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

public partial class Employee : Directory
{
    [ObservableProperty]
    private Guid? personId;

    [ObservableProperty]
    private Guid? postId;

    [ObservableProperty]
    private string? phone;

    [ObservableProperty]
    private string? email;

    [EnumType("employee_role")]
    public string EmpRole { get; set; } = "not defined";

    [DenyWriting]
    public EmployeeRole EmployeeRole
    {
        get { return Enum.Parse<EmployeeRole>(EmpRole.Dehumanize()); }
        protected set { EmpRole = value.ToString().Humanize(LetterCasing.LowerCase); }
    }
}
