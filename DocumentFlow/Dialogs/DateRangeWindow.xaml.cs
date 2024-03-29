//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using FluentDateTime;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для DateRangeWindow.xaml
/// </summary>
public partial class DateRangeWindow : Window, INotifyPropertyChanged
{
    private int year = 2024;
    private (DateTime DateFrom, DateTime DateTo) dateRange;

    public event PropertyChangedEventHandler? PropertyChanged;

    public DateRangeWindow()
    {
        InitializeComponent();
    }

    public int Year
    {
        get => year;
        set
        {
            if (year != value)
            {
                year = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Year)));
            }
        }
    }

    public bool GetRange(out DateTime dateFrom, out DateTime dateTo)
    {
        if (ShowDialog() == true)
        {
            dateFrom = dateRange.DateFrom;
            dateTo = dateRange.DateTo;
            return true;
        }

        dateFrom = default;
        dateTo = default;
        return false;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button) 
        {
            var tag = button.Tag.ToString();
            if (!string.IsNullOrEmpty(tag))
            {
                var range = int.Parse(tag);
                DateTime dateFrom = default;
                DateTime dateTo = default;
                if (range < 13)
                {
                    dateFrom = new DateTime(year, range, 1).BeginningOfDay();
                    dateTo = dateFrom.EndOfMonth();
                    
                } 
                else 
                {
                    switch (range)
                    {
                        case 21:
                            dateFrom = new DateTime(year, 1, 1).BeginningOfDay();
                            dateTo = dateFrom.EndOfQuarter();
                            break;
                        case 22:
                            dateFrom = new DateTime(year, 4, 1).BeginningOfDay();
                            dateTo = dateFrom.EndOfQuarter();
                            break;
                        case 23:
                            dateFrom = new DateTime(year, 7, 1).BeginningOfDay();
                            dateTo = dateFrom.EndOfQuarter();
                            break;
                        case 24:
                            dateFrom = new DateTime(year, 10, 1).BeginningOfDay();
                            dateTo = dateFrom.EndOfQuarter();
                            break;
                        case 32:
                            dateFrom = new DateTime(year, 1, 1).BeginningOfDay();
                            dateTo = new DateTime(year, 6, 1).EndOfMonth();
                            break;
                        case 33:
                            dateFrom = new DateTime(year, 1, 1).BeginningOfDay();
                            dateTo = new DateTime(year, 9, 1).EndOfMonth();
                            break;
                        case 34:
                            dateFrom = new DateTime(year, 1, 1).BeginningOfDay();
                            dateTo = dateFrom.EndOfYear();
                            break;
                    }
                }

                dateRange = (dateFrom, dateTo);

                DialogResult = true;
            }
        }
    }

    private void ButtonSelectDay_Click(object sender, RoutedEventArgs e)
    {
        SelectDayWindow window = new();
        if (window.GetDay(out var day))
        {
            dateRange = (day.BeginningOfDay(), day.EndOfDay());
            DialogResult = true;
        }
    }

    private void ButtonPrevYear_Click(object sender, RoutedEventArgs e)
    {
        Year--;
    }

    private void ButtonNextYear_Click(object sender, RoutedEventArgs e)
    {
        Year++;
    }

    private void DateRangeWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }
}
