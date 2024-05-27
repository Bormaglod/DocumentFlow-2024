//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common;
using DocumentFlow.Common.Enums;
using DocumentFlow.Dialogs;
using DocumentFlow.Messages;
using DocumentFlow.Models.Entities;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DocumentFlow.Controls;

/// <summary>
/// Логика взаимодействия для SelectBox.xaml
/// </summary>
public partial class SelectBox : UserControl
{
    private readonly ObservableCollection<GridComboColumn> columns = new();

    public SelectBox()
    {
        InitializeComponent();

        columns.CollectionChanged += Columns_CollectionChanged;
    }

    public Guid? SelectedValue
    {
        get => (Guid?)GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    public DocumentInfo SelectedItem
    {
        get => (DocumentInfo)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public IEnumerable<DocumentInfo> ItemsSource
    {
        get => (IEnumerable<DocumentInfo>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public TypeContent TypeContent
    {
        get => (TypeContent)GetValue(TypeContentProperty);
        set => SetValue(TypeContentProperty, value);
    }

    public bool CanSelectFolder 
    {
        get => (bool)GetValue(CanSelectFolderProperty);
        set => SetValue(CanSelectFolderProperty, value);
    }

    public Type EditorType
    {
        get => (Type)GetValue(EditorTypeProperty);
        set => SetValue(EditorTypeProperty, value);
    }

    public bool ShowClearButton
    {
        get => (bool)GetValue(ShowClearButtonProperty);
        set => SetValue(ShowClearButtonProperty, value);
    }

    public bool ShowSelectButton
    {
        get => (bool)GetValue(ShowSelectButtonProperty);
        set => SetValue(ShowSelectButtonProperty, value);
    }

    public ICommand ItemSelected
    {
        get => (ICommand)GetValue(ItemSelectedProperty);
        set => SetValue(ItemSelectedProperty, value);
    }

    public bool IsEnabledEditor
    {
        get => (bool)GetValue(IsEnabledEditorProperty);
        set => SetValue(IsEnabledEditorProperty, value);
    }

    public ObservableCollection<GridComboColumn> Columns => columns;

    public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(
        nameof(SelectedValue),
        typeof(Guid?),
        typeof(SelectBox),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(object),
        typeof(SelectBox),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource), 
        typeof(IEnumerable<DocumentInfo>), 
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

    public static readonly DependencyProperty ShowClearButtonProperty = DependencyProperty.Register(
        nameof(ShowClearButton),
        typeof(bool),
        typeof(SelectBox),
        new FrameworkPropertyMetadata(true));

    public static readonly DependencyProperty ShowSelectButtonProperty = DependencyProperty.Register(
        nameof(ShowSelectButton),
        typeof(bool),
        typeof(SelectBox),
        new FrameworkPropertyMetadata(true));

    public static readonly DependencyProperty ItemSelectedProperty = DependencyProperty.Register(
        nameof(ItemSelected),
        typeof(ICommand),
        typeof(SelectBox));

    public static readonly DependencyProperty IsEnabledEditorProperty = DependencyProperty.Register(
        nameof(IsEnabledEditor),
        typeof(bool),
        typeof(SelectBox),
        new FrameworkPropertyMetadata(true));

    private void Columns_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add) 
        { 
            if (e.NewItems != null && e.NewItems.Count > 0 && e.NewItems[0] is GridComboColumn info && info.Order == 0) 
            {
                info.Order = columns.Max(x => x.Order) + 1;
            }
        }
    }

    private void SelectDirectoryValue()
    {
        DirectoryWindow window = new()
        {
            ItemsSource = ItemsSource,
            CanSelectFolder = CanSelectFolder,
            Columns = Columns
        };

        var id = SelectedItem?.Id ?? SelectedValue;
        if (id != null)
        {
            var item = ItemsSource.FirstOrDefault(x => x.Id == id);
            if (item is Directory info)
            {
                window.SelectedItem = info;
            }
        }

        if (window.ShowDialog() == true) 
        {
            SelectedItem = window.SelectedItem!;
            SelectedValue = window.SelectedItem?.Id;
            ItemSelected?.Execute(SelectedItem);
        }
    }

    private void SelectDocumentValue()
    {
        DocumentWindow window = new()
        {
            ItemsSource = ItemsSource,
            Columns = Columns
        };

        var id = SelectedItem?.Id ?? SelectedValue;
        if (id != null)
        {
            var item = ItemsSource.FirstOrDefault(x => x.Id == id);
            if (item is BaseDocument info)
            {
                window.SelectedItem = info;
            }
        }

        if (window.ShowDialog() == true)
        {
            SelectedItem = window.SelectedItem!;
            SelectedValue = window.SelectedItem?.Id;
            ItemSelected?.Execute(SelectedItem);
        }
    }

    private void ButtonClear_Click(object sender, RoutedEventArgs e)
    {
        SelectedValue = null;
        SelectedItem = null!;
    }

    private void ButtonSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ItemsSource == null)
        {
            return;
        }

        switch (TypeContent)
        {
            case TypeContent.Directory:
                SelectDirectoryValue();
                break;
            case TypeContent.Document:
                SelectDocumentValue();
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
