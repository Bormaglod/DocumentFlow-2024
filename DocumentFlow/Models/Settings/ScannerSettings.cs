//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;
using DocumentFlow.Scanner.Enums;

namespace DocumentFlow.Models.Settings;

public class ScannerSettings
{
    public WindowSettings Settings { get; set; } = new();
    public FileExtension ImageStore { get; set; } = FileExtension.Jpg;
    public int Dpi { get; set; } = 300;
    public WIAColorMode ColorMode { get; set; } = WIAColorMode.Color;

    public static int Brightness { get; set; } = 0;
    public static int Contrast { get; set; } = 0;
}