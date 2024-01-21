//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Git: https://github.com/mephraim/ghostscriptsharp
// Date: 24.09.2023
//-----------------------------------------------------------------------

namespace DocumentFlow.Ghostscript.Settings;

/// <summary>
/// Which pages to output
/// </summary>
public class GhostscriptPages
{
    private bool _allPages = true;
    private int _start;
    private int _end;

    /// <summary>
    /// Output all pages avaialble in document
    /// </summary>
    public bool AllPages
    {
        set => (_allPages, _start, _end) = (true, -1, -1);
        get => _allPages;
    }

    /// <summary>
    /// Start output at this page (1 for page 1)
    /// </summary>
    public int Start
    {
        set => (_allPages, _start) = (false, value);
        get => _start;
    }

    /// <summary>
    /// Page to stop output at
    /// </summary>
    public int End
    {
        set => (_allPages, _end) = (false, value);
        get => _end;
    }
}



