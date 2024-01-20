//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common;

using System.ComponentModel;
using System.Windows;

namespace DocumentFlow;

public static class ViewModelLocator
{
    /// <summary>
    /// Gets AutoWireViewModel attached property
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetAutoWireViewModel(DependencyObject obj)
    {
        return (bool)obj.GetValue(AutoWireViewModelProperty);
    }

    /// <summary>
    /// Sets AutoWireViewModel attached property
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetAutoWireViewModel(DependencyObject obj, bool value)
    {
        obj.SetValue(AutoWireViewModelProperty, value);
    }

    /// <summary>
    /// AutoWireViewModel attached property
    /// </summary>
    public static readonly DependencyProperty AutoWireViewModelProperty =
        DependencyProperty.RegisterAttached("AutoWireViewModel",
        typeof(bool), typeof(ViewModelLocator),
        new PropertyMetadata(false, AutoWireViewModelChanged));

    /// <summary>
    /// Step 5 approach to hookup View with ViewModel
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void AutoWireViewModelChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (DesignerProperties.GetIsInDesignMode(d))
        {
            return;
        }

        // Get the type of View
        var viewType = d.GetType(); 

        // Get the name of ViewModel
        string viewModelTypeName;
        if (viewType.Name.EndsWith("View"))
        {
            viewModelTypeName = viewType.FullName!.Replace("Views", "ViewModels") + "Model";
        }
        else
        {
            viewModelTypeName = $"DocumentFlow.ViewModels.{viewType.Name}ViewModel";
        }

        // Get the type of ViewModel
        var viewModelType = Type.GetType(viewModelTypeName); 
        if (viewModelType == null)
        {
            return;
        }

        // Create an instance of ViewModel
        var viewModel = ServiceLocator.Context.GetService(viewModelType);

        // View's DataContext is set to ViewModel
        ((FrameworkElement)d).DataContext = viewModel;
    }
}