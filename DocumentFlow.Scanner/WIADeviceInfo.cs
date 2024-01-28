//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Scanner;

public struct WIADeviceInfo : IEquatable<WIADeviceInfo>
{
    public string DeviceID;
    public string Name;

    public WIADeviceInfo(string DeviceID, string Name)
    {
        this.DeviceID = DeviceID;
        this.Name = Name;
    }

    public readonly bool Equals(WIADeviceInfo other)
    {
        return Equals(other, this);
    }

    public override readonly bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var objectToCompareWith = (WIADeviceInfo)obj;
        return objectToCompareWith.DeviceID == DeviceID && objectToCompareWith.Name == Name;

    }

    public override readonly int GetHashCode()
    {
        return DeviceID.GetHashCode() ^ Name.GetHashCode();
    }

    public static bool operator == (WIADeviceInfo info1, WIADeviceInfo info2)
    {
        return info1.Equals(info2);
    }
    public static bool operator != (WIADeviceInfo info1, WIADeviceInfo info2)
    {
        return !info1.Equals(info2);
    }

    public override readonly string ToString() => Name;
}
