//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// From: https://programmerall.com/article/31292230217/
//-----------------------------------------------------------------------

using Microsoft.Xaml.Behaviors;

using System.Windows;
using System.Windows.Input;

namespace DocumentFlow.Common;

public class EventCommandAction : TriggerAction<DependencyObject>
{
    /// <summary>
    /// Event to be bound command
    /// </summary>
    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MsgName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("Command", typeof(ICommand), typeof(EventCommandAction), new PropertyMetadata(null));

    /// <summary>
    /// The parameters of the bound command are kept empty is the parameters of the event.
    /// </summary>
    public object CommandParameter
    {
        get { return (object)GetValue(CommandParateterProperty); }
        set { SetValue(CommandParateterProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CommandParateter.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CommandParateterProperty =
        DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventCommandAction), new PropertyMetadata(null));

    // Execute an event
    protected override void Invoke(object parameter)
    {
        if (CommandParameter != null)
        {
            parameter = CommandParameter;
        }

        var cmd = Command;
        if (cmd != null && cmd.CanExecute(parameter))
        {
            cmd.Execute(parameter);
        }
    }
}