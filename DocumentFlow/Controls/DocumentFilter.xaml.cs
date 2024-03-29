//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DocumentFlow.Controls;

/// <summary>
/// Логика взаимодействия для DocumentFilter.xaml
/// </summary>
public partial class DocumentFilter : UserControl, INotifyPropertyChanged
{
    private IEnumerable<Organization>? organizations;

    public DocumentFilter()
    {
        InitializeComponent();

        if (!DesignerProperties.GetIsInDesignMode(this))
        {
            using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

            Organizations = conn.Query<Organization>("select id, code, item_name, default_org from organization");
            SelectedOrganization = Organizations.FirstOrDefault(x => x.DefaultOrg);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IEnumerable<Organization>? Organizations
    {
        get => organizations;
        set
        {
            if (organizations != value)
            {
                organizations = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Organizations)));
            }
        }
    }

    public Organization? SelectedOrganization
    {
        get { return (Organization?)GetValue(SelectedOrganizationProperty); }
        set { SetValue(SelectedOrganizationProperty, value); }
    }

    public DateTime? DateRangeFrom
    {
        get { return (DateTime?)GetValue(DateRangeFromProperty); }
        set { SetValue(DateRangeFromProperty, value); }
    }

    public DateTime? DateRangeTo
    {
        get { return (DateTime?)GetValue(DateRangeToProperty); }
        set { SetValue(DateRangeToProperty, value); }
    }

    public ICommand ApplyFilter
    {
        get { return (ICommand)GetValue(ApplyFilterProperty); }
        set { SetValue(ApplyFilterProperty, value); }
    }

    public bool IsVisibleStates
    {
        get => (bool)GetValue(IsVisibleStatesProperty);
        set => SetValue(IsVisibleStatesProperty, value);
    }

    public IEnumerable<State> States
    {
        get => (IEnumerable<State>)GetValue(StatesProperty);
        set => SetValue(StatesProperty, value);
    }

    public State SelectedState
    {
        get => (State)GetValue(SelectedStateProperty);
        set => SetValue(SelectedStateProperty, value);
    }

    public static readonly DependencyProperty SelectedOrganizationProperty = DependencyProperty.Register(
        nameof(SelectedOrganization),
        typeof(Organization),
        typeof(DocumentFilter),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty DateRangeFromProperty = DependencyProperty.Register(
        nameof(DateRangeFrom),
        typeof(DateTime?),
        typeof(DocumentFilter),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty DateRangeToProperty = DependencyProperty.Register(
        nameof(DateRangeTo),
        typeof(DateTime?),
        typeof(DocumentFilter),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty ApplyFilterProperty = DependencyProperty.Register(
        nameof(ApplyFilter), 
        typeof(ICommand), 
        typeof(DocumentFilter));

    public static readonly DependencyProperty IsVisibleStatesProperty = DependencyProperty.Register(
        nameof(IsVisibleStates),
        typeof(bool),
        typeof(DocumentFilter));

    public static readonly DependencyProperty StatesProperty = DependencyProperty.Register(
        nameof(States),
        typeof(IEnumerable<State>),
        typeof(DocumentFilter));

    public static readonly DependencyProperty SelectedStateProperty = DependencyProperty.Register(
        nameof(SelectedState),
        typeof(State),
        typeof(DocumentFilter));

    private void ButtonApply_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter?.Execute(null);
    }

    private void ButtonSelectDateRange_Click(object sender, RoutedEventArgs e)
    {
        DateRangeWindow window = new();
        if (window.GetRange(out var dateFrom, out var dateTo)) 
        {
            DateRangeFrom = dateFrom;
            DateRangeTo = dateTo;
        }
    }
}
