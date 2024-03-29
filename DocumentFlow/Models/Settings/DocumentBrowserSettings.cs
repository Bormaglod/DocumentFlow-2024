//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace DocumentFlow.Models.Settings;

public class DocumentBrowserSettings
{
    [DefaultValue(null)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? DateFrom { get; set; }

    [DefaultValue(null)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? DateTo { get; set; }

    [DefaultValue(0)]
    public int State { get; set; }
}
