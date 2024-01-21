//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Git: https://github.com/mephraim/ghostscriptsharp
// Date: 24.09.2023
//-----------------------------------------------------------------------

using DocumentFlow.Ghostscript.Enums;

using System.Drawing;

namespace DocumentFlow.Ghostscript.Settings;

/// <summary>
/// Output document physical dimensions
/// </summary>
public class GhostscriptPageSize
{
    private GhostscriptPageSizes _fixed;
    private Size _manual;

    /// <summary>
    /// Custom document size
    /// </summary>
    public Size Manual
    {
        set
        {
            _fixed = GhostscriptPageSizes.UNDEFINED;
            _manual = value;
        }
        get
        {
            return _manual;
        }
    }

    /// <summary>
    /// Standard paper size
    /// </summary>
    public GhostscriptPageSizes Native
    {
        set
        {
            _fixed = value;
            _manual = new Size(0, 0);
        }
        get
        {
            return _fixed;
        }
    }

}