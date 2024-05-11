//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Syncfusion.Windows.Shared;

namespace DocumentFlow.Common;

public class DateTimeGridComboColumn : GridComboColumn
{
    public DateTimePattern Pattern { get; set; } = DateTimePattern.ShortDate;
    public string CustomPattern { get; set; } = string.Empty;
}
