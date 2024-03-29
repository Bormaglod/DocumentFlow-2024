//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Syncfusion.Windows.Shared;

using System.Windows;
using System.Windows.Input;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для SelectDayWindow.xaml
/// </summary>
public partial class SelectDayWindow : Window
{
    public SelectDayWindow()
    {
        InitializeComponent();
    }

    public bool GetDay(out DateTime date)
    {
        if (ShowDialog() == true)
        {
            date = calendar.Date;
            return true;
        }

        date = default;
        return false;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void Calendar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is DayCell && e.ChangedButton == MouseButton.Left)
        {
            DialogResult = true;
        }
    }

    private void TextToday_MouseDown(object sender, MouseButtonEventArgs e)
    {
        calendar.Date = DateTime.Now;
        DialogResult = true;
    }
}
