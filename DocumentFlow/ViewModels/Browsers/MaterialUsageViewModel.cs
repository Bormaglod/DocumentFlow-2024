//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;
using DocumentFlow.Views.Editors;

using Microsoft.Extensions.Configuration;

using Syncfusion.Windows.Shared;

using System.Data;
using System.Windows.Input;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class MaterialUsageViewModel : EntityGridViewModel<MaterialUsage>, ISelfTransientLifetime
{
    public MaterialUsageViewModel() { }

    public MaterialUsageViewModel(IDatabase database, IConfiguration configuration) : base(database, configuration) { }

    #region Commands

    #region OpenGoods

    private ICommand? openGoods;

    public ICommand OpenGoods
    {
        get
        {
            openGoods ??= new DelegateCommand(OnOpenGoods);
            return openGoods;
        }
    }

    private void OnOpenGoods(object parameter)
    {
        if (SelectedItem is MaterialUsage item && item.Goods != null) 
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(typeof(GoodsView), item.Goods));
        }
    }

    #endregion

    #region OpenCalculation

    private ICommand? openCalculation;

    public ICommand OpenCalculation
    {
        get
        {
            openCalculation ??= new DelegateCommand(OnOpenCalculation);
            return openCalculation;
        }
    }

    private void OnOpenCalculation(object parameter)
    {
        if (SelectedItem is MaterialUsage item && item.Calculation != null)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(typeof(CalculationView), item.Calculation));
        }
    }

    #endregion

    #endregion

    protected override IReadOnlyList<MaterialUsage> GetData(IDbConnection connection, Guid? id = null)
    {
        return connection.GetQuery<CalculationMaterial>(new QueryParameters() { Quantity = QuantityInformation.None })
            .Select("t0.{id, amount}")
            .Select("t0.item_id as owner_id")
            .MappingQuery<MaterialUsage>(x => x.Calculation)
            .MappingQuery<MaterialUsage>(x => x.Goods)
            .WhereRaw("t1.state = 'approved'::calculation_state")
            .Where("t0.item_id", Owner?.Id)
            .Get<MaterialUsage, Calculation, Goods>(
                map: (usage, calculation, goods) =>
                {
                    usage.Calculation = calculation;
                    usage.Goods = goods;
                    return usage;
                })
            .ToList();
    }

    protected override void InitializeToolBar(IDatabase? database = null)
    {
        ToolBarItems.AddButtons(this,
            new ToolBarButtonModel("Изделие", "goods") { Command = OpenGoods },
            new ToolBarButtonModel("Калькуляция", "calculation") { Command = OpenCalculation },
            new ToolBarSeparatorModel(),
            new ToolBarButtonComboModel("Печать", "print"),
            new ToolBarButtonModel("Настройки", "settings"));
    }
}
