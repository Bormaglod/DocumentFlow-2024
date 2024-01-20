//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models.Settings;

public class ConnectionSettings
{
    public string Name { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Database { get; set; } = string.Empty;
}
