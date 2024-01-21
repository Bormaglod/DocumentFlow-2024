//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Date: 22.11.2023
//-----------------------------------------------------------------------

namespace DocumentFlow.Scanner;

public struct WIADeviceInfo
{
    public string DeviceID;
    public string Name;

    public WIADeviceInfo(string DeviceID, string Name)
    {
        this.DeviceID = DeviceID;
        this.Name = Name;
    }

    public override readonly string ToString() => Name;
}
