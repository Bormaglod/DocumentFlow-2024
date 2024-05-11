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
/// Логика взаимодействия для OperationsPerformedView.xaml
/// </summary>
public partial class OperationsPerformedView : UserControl, IGridPageView, ISelfTransientLifetime
{
    public OperationsPerformedView()
    {
        InitializeComponent();
        columnOrder.AllowGrouping = true;
        columnLot.AllowGrouping = true;
        columnGoodsCode.AllowGrouping = true;
        columnGoodsItemName.AllowGrouping = true;
    }

    public SfDataGrid DataGrid => gridContent;

    public object Owner
    {
        get => GetValue(OwnerProperty);
        set => SetValue(OwnerProperty, value);
    }

    public bool AvailableGrouping
    {
        get => (bool)GetValue(AvailableGroupingProperty);
        set => SetValue(AvailableGroupingProperty, value);
    }

    public SizeMode SizeMode
    {
        get => (SizeMode)GetValue(SizeModeProperty);
        set => SetValue(SizeModeProperty, value);
    }

    public bool AllowFiltering
    {
        get => (bool)GetValue(AllowFilteringProperty);
        set => SetValue(AllowFilteringProperty, value);
    }

    public bool IsGroupDropAreaExpanded 
    {
        get => (bool)GetValue(IsGroupDropAreaExpandedProperty);
        set => SetValue(IsGroupDropAreaExpandedProperty, value);
    }

    public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register(
        nameof(Owner),
        typeof(object),
        typeof(OperationsPerformedView),
        new FrameworkPropertyMetadata(OnOwnerChanged));


    public static readonly DependencyProperty AvailableGroupingProperty = DependencyProperty.Register(
        nameof(AvailableGrouping),
        typeof(bool),
        typeof(OperationsPerformedView),
        new FrameworkPropertyMetadata(true, OnAvailableGroupingChanged));

    public static readonly DependencyProperty SizeModeProperty = DependencyProperty.Register(
        nameof(SizeMode),
        typeof(SizeMode),
        typeof(OperationsPerformedView),
        new FrameworkPropertyMetadata(SizeMode.Normal, OnSizeModeChanged));

    public static readonly DependencyProperty AllowFilteringProperty = DependencyProperty.Register(
        nameof(AllowFiltering),
        typeof(bool),
        typeof(OperationsPerformedView),
        new FrameworkPropertyMetadata(true, OnAllowFilteringChanged));

    public static readonly DependencyProperty IsGroupDropAreaExpandedProperty = DependencyProperty.Register(
        nameof(IsGroupDropAreaExpanded),
        typeof(bool),
        typeof(OperationsPerformedView),
        new FrameworkPropertyMetadata(true, OnIsGroupDropAreaExpandedChanged));

    private static void OnOwnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is OperationsPerformedView view && view.DataContext is IEntityGridViewModel model)
        {
            model.Owner = e.NewValue as DocumentInfo;

            view.columnOrder.AllowGrouping = view.Owner == null;
            view.columnLot.AllowGrouping = view.Owner == null;
            view.columnGoodsCode.AllowGrouping = view.Owner == null;
            view.columnGoodsItemName.AllowGrouping = view.Owner == null;
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

    private static void OnAllowFilteringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement view && view.DataContext is IDocumentGridViewModel model)
        {
            model.AllowFiltering = (bool)e.NewValue;
        }
    }

    private static void OnIsGroupDropAreaExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement view && view.DataContext is IEntityGridViewModel model)
        {
            model.IsGroupDropAreaExpanded = (bool)e.NewValue;
        }
    }
}
