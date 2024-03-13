//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Data;

using Humanizer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Models.Entities;

public partial class Contractor : Company
{
    [ObservableProperty]
    private Person? person;

    [DenyWriting]
    public SubjectsCivilLow? SubjectCivilLow
    {
        get { return Subject == null ? null : Enum.Parse<SubjectsCivilLow>(Subject.Dehumanize()); }
        set { Subject = value?.ToString().Humanize(LetterCasing.LowerCase); }
    }

    [EnumType("subjects_civil_low")]
    public string? Subject { get; set; }

    public override string ToString() => Code;
}
