//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models.Settings;

public class GroupSettings
{
    public int Order { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Extended { get; set; }
}
