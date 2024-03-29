//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Models.Settings;

using System.Windows;

namespace DocumentFlow.ViewModels;

public partial class WindowViewModel : ObservableObject
{
    [ObservableProperty]
    private double left;

    [ObservableProperty]
    private double top;

    [ObservableProperty]
    private double width = 800;

    [ObservableProperty]
    private double height = 450;

    [ObservableProperty]
    private WindowState windowState;

    protected void RestoreSettings(WindowSettings settings)
    {
        if (settings.WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }
        else
        {
            WindowState = settings.WindowState;
        }

        if (WindowState == WindowState.Normal)
        {
            Left = settings.Left;
            Top = settings.Top; ;

            Width = settings.Width;
            Height = settings.Height;
        }
    }

    protected void SaveSettings(WindowSettings settings)
    {
        settings.WindowState = WindowState;
        settings.Left = Left;
        settings.Top = Top;
        settings.Width = Width;
        settings.Height = Height;
    }
}
