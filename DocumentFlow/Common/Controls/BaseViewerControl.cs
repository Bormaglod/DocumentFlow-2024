//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Tools.Controls;

using System.Windows;
using System.Windows.Controls;

namespace DocumentFlow.Common.Controls;

public class BaseViewerControl : UserControl, IGridPageView
{
    private readonly GridRowSizingOptions gridRowResizingOptions = new();

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

    public bool IsDependent
    {
        get => (bool)GetValue(IsDependentProperty);
        set => SetValue(IsDependentProperty, value);
    }

    public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register(
        nameof(Owner),
        typeof(object),
        typeof(BaseViewerControl),
        new FrameworkPropertyMetadata(OnOwnerChanged));

    public static readonly DependencyProperty AvailableNavigationProperty = DependencyProperty.Register(
        nameof(AvailableNavigation),
        typeof(bool),
        typeof(BaseViewerControl),
        new FrameworkPropertyMetadata(true, OnAvailableNavigationChanged));

    public static readonly DependencyProperty AvailableGroupingProperty = DependencyProperty.Register(
        nameof(AvailableGrouping),
        typeof(bool),
        typeof(BaseViewerControl),
        new FrameworkPropertyMetadata(true, OnAvailableGroupingChanged));

    public static readonly DependencyProperty SizeModeProperty = DependencyProperty.Register(
        nameof(SizeMode),
        typeof(SizeMode),
        typeof(BaseViewerControl),
        new FrameworkPropertyMetadata(SizeMode.Normal, OnSizeModeChanged));

    public static readonly DependencyProperty AllowFilteringProperty = DependencyProperty.Register(
        nameof(AllowFiltering),
        typeof(bool),
        typeof(BaseViewerControl),
        new FrameworkPropertyMetadata(true, OnAllowFilteringChanged));

    public static readonly DependencyProperty IsGroupDropAreaExpandedProperty = DependencyProperty.Register(
        nameof(IsGroupDropAreaExpanded),
        typeof(bool),
        typeof(BaseViewerControl),
        new FrameworkPropertyMetadata(true, OnIsGroupDropAreaExpandedChanged));

    public static readonly DependencyProperty IsDependentProperty = DependencyProperty.Register(
        nameof(IsDependent),
        typeof(bool),
        typeof(BaseViewerControl),
        new FrameworkPropertyMetadata(false, OnIsDependentChanged));

    public SfDataGrid? GetDataGrid()
    {
        return this.FindChild<SfDataGrid>("gridContent");
    }

    protected virtual void OnOwnerChanged(object newValue) { }

    protected void GridContent_QueryRowHeight(object sender, QueryRowHeightEventArgs e)
    {
        if (sender is not SfDataGrid grid)
        {
            return;
        }

        if (grid.GetHeaderIndex() == e.RowIndex)
        {
            if (grid.GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out var autoHeight))
            {
                if (autoHeight > 24)
                {
                    e.Height = autoHeight;
                    e.Handled = true;
                }
            }
        }
    }

    private static void OnOwnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BaseViewerControl view && view.DataContext is IEntityGridViewModel model)
        {
            model.Owner = e.NewValue as DocumentInfo;
            view.OnOwnerChanged(e.NewValue);
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

    private static void OnIsDependentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement view && view.DataContext is IEntityGridViewModel model)
        {
            model.IsDependent = (bool)e.NewValue;
        }
    }
}
