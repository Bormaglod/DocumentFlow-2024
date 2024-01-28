//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Scanner.Enums;

namespace DocumentFlow.Scanner;

public class ScanSettings
{
    public WIAColorMode ColorMode { get; set; } = WIAColorMode.Color;
    public int Resolution { get; set; } = 300;
    public WIAPageSize PageSize { get; set; } = WIAPageSize.A4;
    public int Brightness { get; set; } = 0;
    public int Contrast { get; set; } = 0;
}
