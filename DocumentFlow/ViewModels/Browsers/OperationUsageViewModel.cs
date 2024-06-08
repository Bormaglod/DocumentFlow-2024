//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Messages.Options;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;
using DocumentFlow.Views.Editors;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public sealed partial class OperationUsageViewModel : EntityGridViewModel<OperationUsage>, ISelfTransientLifetime
{
    public OperationUsageViewModel() { }

    public OperationUsageViewModel(IDatabase database, IConfiguration configuration, ILogger<OperationUsageViewModel> logger) : base(database, configuration, logger) { }

    #region Commands

    [RelayCommand]
    private void OpenGoods()
    {
        if (SelectedItem is OperationUsage usage && usage.Goods != null)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(typeof(GoodsView), usage.Goods));
        }
    }

    [RelayCommand]
    private void OpenCalculation()
    {
        if (SelectedItem is OperationUsage usage && usage.Calculation != null && usage.Goods != null)
        {
            WeakReferenceMessenger.Default.Send(
                new EntityEditorOpenMessage(
                    typeof(CalculationView), 
                    usage.Calculation) 
                { 
                    Options = new DocumentEditorMessageOptions(usage.Goods) 
                });
        }
    }

    #endregion

    protected override bool GetSupportAccepting() => false;

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Goods))
        {
            columnInfo.State = ColumnVisibleState.AlwaysVisible;
        }
    }

    protected override IReadOnlyList<OperationUsage> GetData(IDbConnection connection, Guid? id = null)
    {
        var parameters = new QueryParameters()
        {
            Alias = "co",
            Table = "calculation_operation",
            Quantity = QuantityInformation.None,
            IncludeDocumentsInfo = false,
            OwnerIdName = "item_id"
        };

        return DefaultQuery(connection, id, parameters)
            .Select("c.id")
            .Select("co.item_id as owner_id")
            .SelectRaw("sum(co.repeats) as amount")
            .MappingQuery<OperationUsage>(x => x.Calculation, QuantityInformation.Directory, "c")
            .MappingQuery<OperationUsage>(x => x.Goods, QuantityInformation.Directory, "g")
            .WhereRaw("c.state = 'approved'::calculation_state")
            .GroupBy("c.id", "co.item_id", "c.item_name", "c.code", "g.id", "g.code", "g.item_name")
            .Get<OperationUsage, Calculation, Goods>(
                map: (usage, calculation, goods) =>
                {
                    usage.Goods = goods;
                    usage.Calculation = calculation;
                    return usage;
                }
            )
            .ToList();
    }

    protected override void InitializeToolBar()
    {
        ToolBarItems.AddButtons(this,
            new ToolBarButtonModel("Изделие", "goods") { Command = OpenGoodsCommand },
            new ToolBarButtonModel("Калькуляция", "calculation") { Command = OpenCalculationCommand });
    }
}
