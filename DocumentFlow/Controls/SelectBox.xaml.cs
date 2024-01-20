//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common.Enums;
using DocumentFlow.Dialogs;
using DocumentFlow.Messages;
using DocumentFlow.Models.Entities;

using System.Windows;
using System.Windows.Controls;

namespace DocumentFlow.Controls;

/// <summary>
/// Логика взаимодействия для SelectBox.xaml
/// </summary>
public partial class SelectBox : UserControl
{
    public SelectBox()
    {
        InitializeComponent();
    }

    public Guid? SelectedValue
    {
        get { return (Guid?)GetValue(SelectedValueProperty); }
        set { SetValue(SelectedValueProperty, value); }
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

    public string SelectedText
    {
        get { return (string)GetValue(SelectedTextProperty); }
        set { SetValue(SelectedTextProperty, value); }
    }

    public TypeContent TypeContent
    {
        get { return (TypeContent)GetValue(TypeContentProperty); }
        set { SetValue(TypeContentProperty, value); }
    }

    public bool CanSelectFolder 
    {
        get { return (bool)GetValue(CanSelectFolderProperty); }
        set { SetValue(CanSelectFolderProperty, value); }
    }

    public Type EditorType
    {
        get { return (Type)GetValue(EditorTypeProperty); }
        set { SetValue(EditorTypeProperty, value); }
    }

    public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(
        nameof(SelectedValue),
        typeof(Guid?),
        typeof(SelectBox),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedValueChanged));

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(object),
        typeof(SelectBox),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource), 
        typeof(IEnumerable<DocumentInfo>), 
        typeof(SelectBox),
        new FrameworkPropertyMetadata(OnItemsSourceChanged));

    public static readonly DependencyProperty SelectedTextProperty = DependencyProperty.Register(
        nameof(SelectedText),
        typeof(string),
        typeof(SelectBox));

    public static readonly DependencyProperty TypeContentProperty = DependencyProperty.Register(
        nameof(TypeContent),
        typeof(TypeContent),
        typeof(SelectBox));

    public static readonly DependencyProperty CanSelectFolderProperty = DependencyProperty.Register(
        nameof(CanSelectFolder),
        typeof(bool),
        typeof(SelectBox));

    public static readonly DependencyProperty EditorTypeProperty = DependencyProperty.Register(
        nameof(EditorType),
        typeof(Type),
        typeof(SelectBox));

    private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SelectBox selectBox)
        {
            if (e.NewValue is Guid id)
            {
                selectBox.SelectedItem = selectBox.ItemsSource.FirstOrDefault(x => x.Id == id)!;
            }
            else
            {
                selectBox.SelectedItem = null!;
            }
        }
    }

    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SelectBox selectBox)
        {
            selectBox.SelectedText = e.NewValue?.ToString() ?? string.Empty;
        }
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SelectBox selectBox)
        {
            if (selectBox.SelectedItem != null)
            {
                selectBox.SelectedText = selectBox.SelectedItem.ToString() ?? string.Empty;
            }
            else
            {
                selectBox.SelectedText = string.Empty;
            }
        }
    }

    private void SelectDirectoryValue()
    {
        DirectoryWindow window = new()
        {
            ItemsSource = ItemsSource.OfType<Directory>(),
            CanSelectFolder = CanSelectFolder
        };

        if (SelectedItem != null)
        {
            var item = ItemsSource.FirstOrDefault(x => x.Id == SelectedItem.Id);
            if (item is Directory info)
            {
                window.SelectedItem = info;
            }
        }

        if (window.ShowDialog() == true) 
        {
            SelectedValue = window.SelectedItem.Id;
        }
    }

    private void ButtonClear_Click(object sender, RoutedEventArgs e)
    {
        SelectedValue = null;
    }

    private void ButtonSelect_Click(object sender, RoutedEventArgs e)
    {
        switch (TypeContent)
        {
            case TypeContent.Directory:
                SelectDirectoryValue();
                break;
            case TypeContent.Document:
                break;
        }
    }

    private void ButtonOpenEditor_Click(object sender, RoutedEventArgs e)
    {
        if (EditorType != null && SelectedItem != null)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(EditorType, SelectedItem));
        }
    }
}
