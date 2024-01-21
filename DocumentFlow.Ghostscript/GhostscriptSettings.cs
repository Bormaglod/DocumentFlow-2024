//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Git: https://github.com/mephraim/ghostscriptsharp
// Date: 24.09.2023
//-----------------------------------------------------------------------

using DocumentFlow.Ghostscript.Enums;
using DocumentFlow.Ghostscript.Settings;

using System.Drawing;

namespace DocumentFlow.Ghostscript;

/// <summary>
/// Ghostscript settings
/// </summary>
public class GhostscriptSettings
{
    public GhostscriptDevices Device { get; set; }

    public GhostscriptPages Page { get; set; } = new();

    public Size Resolution { get; set; }

    public GhostscriptPageSize Size { get; set; } = new();
}