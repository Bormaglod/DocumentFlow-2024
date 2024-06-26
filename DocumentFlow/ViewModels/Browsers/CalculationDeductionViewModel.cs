﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public sealed partial class CalculationDeductionViewModel : DirectoryViewModel<CalculationDeduction>, ISelfTransientLifetime
{
    public CalculationDeductionViewModel() { }

    public CalculationDeductionViewModel(IDatabase database, IConfiguration configuration, ILogger<CalculationDeductionViewModel> logger) : base(database, configuration, logger) { }

    #region Commands

    [RelayCommand]
    private void ShowDeduction()
    {
        if (SelectedItem is CalculationDeduction op && op.Deduction != null)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(typeof(Views.Editors.DeductionView), op.Deduction));
        }
    }

    #endregion

    public override Type? GetEditorViewType() => typeof(Views.Editors.CalculationDeductionView);

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(CalculationDeduction.Deduction))
        {
            columnInfo.State = ColumnVisibleState.AlwaysVisible;
        }
    }

    protected override void InitializeToolBar()
    {
        base.InitializeToolBar();

        ToolBarItems.AddButtons(this,
            new ToolBarSeparatorModel(),
            new ToolBarButtonModel("Удержание", "discount") { Command = ShowDeductionCommand });
    }

    protected override IReadOnlyList<CalculationDeduction> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id)
            .Get<CalculationDeduction, Deduction>(
                map: (op, deduction) =>
                {
                    op.Deduction = deduction;
                    return op;
                })
            .ToList();
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<CalculationDeduction>(x => x.Deduction);
    }
}
