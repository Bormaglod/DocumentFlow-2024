//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Common;

public class NumericGridComboColumn : GridComboColumn
{
    public int NumberDecimalDigits { get; set; } = 0;
    public bool Grouping { get; set; } = true;
}