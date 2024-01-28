//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Windows;

namespace DocumentFlow.Models.Settings;

public class WindowSettings
{
    public WindowState WindowState { get; set; } = WindowState.Normal;
    public double Left { get; set; } = 0;
    public double Top { get; set; } = 0;
    public double Width { get; set; } = 800;
    public double Height { get; set; } = 600;
}
