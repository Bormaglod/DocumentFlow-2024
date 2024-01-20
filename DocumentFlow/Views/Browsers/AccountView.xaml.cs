//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Tools.Controls;

using System.Windows;
using System.Windows.Controls;

namespace DocumentFlow.Views.Browsers;

/// <summary>
/// Логика взаимодействия для AccountView.xaml
/// </summary>
public partial class AccountView : UserControl, IGridPageView, ISelfTransientLifetime
{
    public AccountView()
    {
        InitializeComponent();
    }

    public SfDataGrid DataGrid => gridContent;

    public object Owner
    {
        get { return GetValue(OwnerProperty); }
        set { SetValue(OwnerProperty, value); }
    }

    public bool AvailableNavigation
    {
        get { return (bool)GetValue(AvailableNavigationProperty); }
        set { SetValue(AvailableNavigationProperty, value); }
    }

    public bool AvailableGrouping
    {
        get { return (bool)GetValue(AvailableGroupingProperty); }
        set { SetValue(AvailableGroupingProperty, value); }
    }

    public SizeMode SizeMode 
    {
        get { return (SizeMode)GetValue(SizeModeProperty); }
        set { SetValue(SizeModeProperty, value); }
    }

    public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register(
        nameof(Owner),
        typeof(object),
        typeof(AccountView),
        new FrameworkPropertyMetadata(OnOwnerChanged));

    public static readonly DependencyProperty AvailableNavigationProperty = DependencyProperty.Register(
        nameof(AvailableNavigation),
        typeof(bool),
        typeof(AccountView),
        new FrameworkPropertyMetadata(true, OnAvailableNavigationChanged));

    public static readonly DependencyProperty AvailableGroupingProperty = DependencyProperty.Register(
        nameof(AvailableGrouping),
        typeof(bool),
        typeof(AccountView),
        new FrameworkPropertyMetadata(true, OnAvailableGroupingChanged));

    public static readonly DependencyProperty SizeModeProperty = DependencyProperty.Register(
        nameof(SizeMode),
        typeof(SizeMode),
        typeof(AccountView),
        new FrameworkPropertyMetadata(SizeMode.Normal, OnSizeModeChanged));

    private static void OnOwnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement view && view.DataContext is IEntityGridViewModel model)
        {
            model.Owner = e.NewValue as DocumentInfo;
        }
    }

    private static void OnAvailableNavigationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement view && view.DataContext is IEntityGridViewModel model)
        {
            model.AvailableNavigation = (bool)e.NewValue;
        }
    }

    private static void OnAvailableGroupingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement view && view.DataContext is IEntityGridViewModel model)
        {
            model.AvailableGrouping = (bool)e.NewValue;
        }
    }

    private static void OnSizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement view && view.DataContext is IEntityGridViewModel model)
        {
            model.SizeMode = (SizeMode)e.NewValue;
        }
    }
}
