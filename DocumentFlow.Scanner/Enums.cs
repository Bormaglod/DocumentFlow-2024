//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Date: 22.11.2023
//-----------------------------------------------------------------------

namespace DocumentFlow.Scanner.Enums;

// https://msdn.microsoft.com/en-us/library/windows/desktop/ms630313(v=vs.85).aspx
public enum WIADeviceInfoProp
{
    DeviceID = 2,
    Manufacturer = 3,
    Description = 4,
    Type = 5,
    Port = 6,
    Name = 7,
    Server = 8,
    RemoteDevID = 9,
    UIClassID = 10,
}

// http://www.papersizes.org/a-paper-sizes.htm
public enum WIAPageSize
{
    A4, // 8.3 x 11.7 in  (210 x 297 mm)
    Letter, // 8.5 x 11 in (216 x 279 mm)
    Legal, // 8.5 x 14 in (216 x 356 mm)
}