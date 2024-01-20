//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/license/mit-0/
// From: https://stackoverflow.com/a/7003338
//-----------------------------------------------------------------------

using Microsoft.Xaml.Behaviors;

using System.Windows;
using System.Windows.Input;

namespace DocumentFlow.Common.Behaviors;

/// <summary>
/// Captures and eats MouseWheel events so that a nested ListBox does not
/// prevent an outer scrollable control from scrolling.
/// </summary>
public sealed class IgnoreMouseWheelBehavior : Behavior<UIElement>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
        base.OnDetaching();
    }

    void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;

        var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = UIElement.MouseWheelEvent
        };

        AssociatedObject.RaiseEvent(e2);
    }
}