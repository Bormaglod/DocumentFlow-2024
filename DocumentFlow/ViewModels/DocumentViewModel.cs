//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common.Converters;
using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;
using DocumentFlow.Models.Settings;

using FluentDateTime;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlKata;

using System.Windows.Data;

namespace DocumentFlow.ViewModels;

public abstract partial class DocumentViewModel<T> : EntityGridViewModel<T>, ICustomGroupingView, IDocumentGridViewModel
    where T : BaseDocument
{
    private IConfigurationSection? filterSection;

    [ObservableProperty]
    private State? selectedState;

    [ObservableProperty]
    private bool allowFiltering = true;

    public DocumentViewModel() { }

    public DocumentViewModel(IDatabase database, IConfiguration configuration, ILogger<DocumentViewModel<T>> logger) : base(database, configuration, logger) { }

    #region Commands

    [RelayCommand]
    private void ApplyFilter()
    {
        if (filterSection != null)
        {
            filterSection["DateFrom"] = DateFrom?.BeginningOfDay().ToString("s");
            filterSection["DateTo"] = DateTo?.EndOfDay().ToString("s");

            RefreshDataSource();
        }
    }

    [RelayCommand]
    private void Accept()
    {
        if (SelectedItem is not T row)
        {
            return;
        }

        ExecuteSystemOperation(row, SystemOperation.Accept, true);
        WeakReferenceMessenger.Default.Send(new DocumentActionMessage<T>((T)SelectedItem));
    }

    [RelayCommand]
    private void CancelAcceptance()
    {
        if (SelectedItem is not T row)
        {
            return;
        }

        ExecuteSystemOperation(row, SystemOperation.Accept, false);
        WeakReferenceMessenger.Default.Send(new DocumentActionMessage<T>((T)SelectedItem));
    }

    #endregion

    public virtual IReadOnlyList<(string MappingName, IValueConverter Converter)> GroupingColumns => new List<(string, IValueConverter)>()
    {
        ( "DocumentDate", new GroupDateTimeConverter("DocumentDateByDate") { Grouping = DateTimeGrouping.ByDate, Text = "По дням" } ),
        ( "DocumentDate", new GroupDateTimeConverter("DocumentDateByMonth") { Grouping = DateTimeGrouping.ByMonth, Text = "По месяцам" } )
    };

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public IEnumerable<State> States { get; } =
    [
        new() { Id = 0, StateName = "Все документы"},
        new() { Id = -1, StateName = "Активные"}
    ];

    protected override bool GetSupportAccepting() => typeof(T).IsAssignableTo(typeof(AccountingDocument));

    protected override void LoadFilter(IConfigurationSection section)
    {
        if (AllowFiltering)
        {
            filterSection = section.GetSection("Filter");
            if (filterSection.Exists())
            {
                DateFrom = filterSection.GetValue<DateTime?>("DateFrom");
                DateTo = filterSection.GetValue<DateTime?>("DateTo");

                var state = filterSection.GetValue<int>("State");
                SelectedState = States.FirstOrDefault(x => x.Id == state);
            }
            else
            {
                DateFrom = DateTime.Now.BeginningOfYear();
                DateTo = DateTime.Now.EndOfYear();
            }
        }
    }

    protected override object? GetFilter()
    {
        if (filterSection != null && AllowFiltering)
        {
            var state = filterSection.GetValue<int>("State");
            return new DocumentBrowserSettings
            {
                DateFrom = filterSection.GetValue<DateTime?>("DateFrom"),
                DateTo = filterSection.GetValue<DateTime?>("DateTo"),
                State = States.FirstOrDefault(x => x.Id == state)?.Id ?? 0
            };
        }

        return base.GetFilter();
    }

    protected override void InitializeToolBar()
    {
        ToolBarItems.AddButtons(this,
            new ToolBarButtonModel("Создать", "file-add") { Command = CreateRowCommand },
            new ToolBarButtonModel("Изменить", "file-edit") { Command = EditCurrentRowCommand },
            new ToolBarButtonModel("Пометить", "file-delete") { Command = SwapMarkedRowCommand },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Удалить", "trash") { Command = WipeRowsCommand },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Копия", "copy-edit") { Command = CopyRowCommand },
            new ToolBarSeparatorModel(),
            new ToolBarButtonComboModel("Печать", "print", Reports),
            new ToolBarButtonModel("Настройки", "settings"));
    }

    protected virtual IEnumerable<short>? GetIncludingStates(int stateCategory) => null;

    protected virtual IEnumerable<short>? GetExcludingStates(int stateCategory)
    {
        return stateCategory switch
        {
            -1 => new[] { State.Canceled, State.Completed },
            _ => null,
        };
    }

    protected override Query FilterQuery(Query query)
    {
        var alias = query.GetOneComponent<AbstractFrom>("from").Alias ?? "t0";

        var filterQuery = new Query();
        /*if (comboOrg.SelectedItem is Organization org)
        {
            query.Where($"{tableName}.organization_id", org.Id);
        }*/

        if (DateFrom.HasValue || DateTo.HasValue)
        {
            if (DateTo.HasValue && DateFrom.HasValue)
            {
                filterQuery.WhereBetween($"{alias}.document_date", DateFrom, DateTo);
            }
            else if (DateTo.HasValue)
            {
                filterQuery.Where($"{alias}.document_date", "<=", DateTo);
            }
            else if (DateFrom.HasValue)
            {
                filterQuery.Where($"{alias}.document_date", ">=", DateFrom);
            }
        }

        if (SelectedState != null && SelectedState.Id != 0)
        {
            var states = GetIncludingStates(SelectedState.Id);
            if (states != null)
            {
                filterQuery.WhereIn($"{alias}.state_id", states);
            }

            states = GetExcludingStates(SelectedState.Id);
            if (states != null)
            {
                filterQuery.WhereNotIn($"{alias}.state_id", states);
            }
        }

        if (filterQuery.Clauses.Count > 0)
        {
            return filterQuery;
        }

        return base.FilterQuery(query);
    }

    partial void OnSelectedStateChanged(State? value)
    {
        if (filterSection != null)
        {
            filterSection["State"] = (value?.Id ?? 0).ToString();
        }

        if (IsLoaded)
        {
            RefreshDataSource();
        }
    }
}
