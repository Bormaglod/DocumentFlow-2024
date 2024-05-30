//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Messages;
using DocumentFlow.Models.Entities;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DocumentFlow.Controls;

/// <summary>
/// Логика взаимодействия для ComboBox.xaml
/// </summary>
[INotifyPropertyChanged]
public partial class ComboBox : UserControl
{
    [ObservableProperty]
    private DocumentInfo? documentItem;

    public ComboBox()
    {
        InitializeComponent();
    }

    public DocumentInfo SelectedItem
    {
        get { return (DocumentInfo)GetValue(SelectedItemProperty); }
        set { SetValue(SelectedItemProperty, value); }
    }

    public IEnumerable<DocumentInfo> ItemsSource
    {
        get { return (IEnumerable<DocumentInfo>)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public Type EditorType
    {
        get { return (Type)GetValue(EditorTypeProperty); }
        set { SetValue(EditorTypeProperty, value); }
    }

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(object),
        typeof(ComboBox),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(IEnumerable<DocumentInfo>),
        typeof(ComboBox),
        new FrameworkPropertyMetadata(OnItemsSourceChanged));

    public static readonly DependencyProperty EditorTypeProperty = DependencyProperty.Register(
        nameof(EditorType),
        typeof(Type),
        typeof(ComboBox));

    private void ButtonClear_Click(object sender, RoutedEventArgs e)
    {
        SelectedItem = null!;
        DocumentItem = null;
    }

    private void ButtonOpenEditor_Click(object sender, RoutedEventArgs e)
    {
        if (EditorType != null && SelectedItem != null) 
        { 
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(EditorType, SelectedItem));
        }
    }

    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ComboBox combo)
        {
            if (e.NewValue is DocumentInfo info)
            {
                combo.DocumentItem ??= combo.ItemsSource?.FirstOrDefault(x => x.Id == info.Id)!;
            }
            else
            {
                combo.DocumentItem = null!;
            }
        }
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ComboBox combo && combo.SelectedItem != null)
        {
            combo.DocumentItem = combo.ItemsSource?.FirstOrDefault(x => x.Id == combo.SelectedItem.Id);
        }
    }

    partial void OnDocumentItemChanged(DocumentInfo? value)
    {
        if (value != SelectedItem) 
        {
            if (value == null)
            {
                SelectedItem = null!;
            }
            else
            {
                if (SelectedItem != null && value.Id != SelectedItem.Id)
                {
                    SelectedItem = value!;
                }
            }
        }
    }
}
