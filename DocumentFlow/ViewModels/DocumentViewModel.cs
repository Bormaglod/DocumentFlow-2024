//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;
using DocumentFlow.Models.Settings;

using FluentDateTime;

using Humanizer;

using Microsoft.Extensions.Configuration;

using SqlKata;

using Syncfusion.UI.Xaml.Utility;

using System.Windows.Input;

namespace DocumentFlow.ViewModels;

public abstract partial class DocumentViewModel<T> : EntityGridViewModel<T>
    where T : BaseDocument
{
    private IConfigurationSection? filterSection;

    [ObservableProperty]
    private State? selectedState;

    public DocumentViewModel() { }

    public DocumentViewModel(IDatabase database, IConfiguration configuration) : base(database, configuration) { }

    #region Commands

    #region ApplyCommand

    private ICommand? applyCommand;

    public ICommand ApplyCommand
    {
        get
        {
            applyCommand ??= new BaseCommand(OnApplyCommand);
            return applyCommand;
        }
    }

    private void OnApplyCommand(object parameter)
    {
        if (filterSection != null)
        {
            filterSection["DateFrom"] = DateFrom?.BeginningOfDay().ToString("s");
            filterSection["DateTo"] = DateTo?.EndOfDay().ToString("s");

            RefreshDataSource();
        }
    }

    #endregion

    #region AcceptCommand

    private ICommand? acceptCommand;

    public ICommand AcceptCommand
    {
        get
        {
            acceptCommand ??= new BaseCommand(OnAcceptCommand);
            return acceptCommand;
        }
    }

    private void OnAcceptCommand(object parameter)
    {
        if (SelectedItem is not T row)
        {
            return;
        }

        var sql = $"call execute_system_operation(:Id, 'accept'::system_operation, true, '{typeof(T).Name.Underscore()}')";
        ExecuteSqlById(sql, row);
    }

    #endregion

    #region CancelAccepanceCommand

    private ICommand? cancelAccepanceCommand;

    public ICommand CancelAccepanceCommand
    {
        get
        {
            cancelAccepanceCommand ??= new BaseCommand(OnCancelAccepanceCommand);
            return cancelAccepanceCommand;
        }
    }

    private void OnCancelAccepanceCommand(object parameter)
    {
        if (SelectedItem is not T row)
        {
            return;
        }

        var sql = $"call execute_system_operation(:Id, 'accept'::system_operation, false, '{typeof(T).Name.Underscore()}')";
        ExecuteSqlById(sql, row);
    }

    #endregion

    #endregion

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public IEnumerable<State> States { get; } = new List<State>()
    {
        new() { Id = 0, StateName = "Все документы"},
        new() { Id = -1, StateName = "Активные"}
    };

    protected override bool GetSupportAccepting() => typeof(T).IsAssignableTo(typeof(AccountingDocument));

    protected override void LoadFilter(IConfigurationSection section)
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

    protected override object? GetFilter()
    {
        if (filterSection != null)
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
            new ToolBarButtonModel("Создать", "file-add") { Command = CreateRow },
            new ToolBarButtonModel("Изменить", "file-edit") { Command = EditCurrentRow },
            new ToolBarButtonModel("Пометить", "file-delete") { Command = SwapMarkedRow },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Удалить", "trash") { Command = WipeRows },
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Копия", "copy-edit") { Command = CopyRow },
            new ToolBarSeparatorModel(),
            new ToolBarButtonComboModel("Печать", "print", Reports),
            new ToolBarButtonModel("Настройки", "settings"));
    }

    protected virtual IEnumerable<int>? GetIncludingStates(int stateCategory) => null;

    protected virtual IEnumerable<int>? GetExcludingStates(int stateCategory)
    {
        return stateCategory switch
        {
            -1 => new[] { 1001, 1002 },
            _ => null,
        };
    }

    protected override Query? FilterQuery(Query query)
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

        return null;
    }

    partial void OnSelectedStateChanged(State? value)
    {
        if (filterSection != null)
        {
            filterSection["State"] = (value?.Id ?? 0).ToString();
        }

        RefreshDataSource();
    }
}
