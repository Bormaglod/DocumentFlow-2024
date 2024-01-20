//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models.Settings;

public class AppSettings
{
    public ConnectionSettings[] Connections { get; set; } = Array.Empty<ConnectionSettings>();
    public bool UseDataNotification { get; set; }
}
