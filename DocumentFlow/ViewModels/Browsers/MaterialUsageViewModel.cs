//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;
using DocumentFlow.Views.Editors;

using Syncfusion.Windows.Shared;

using System.Data;
using System.Windows.Input;

namespace DocumentFlow.ViewModels.Browsers;

public class MaterialUsageViewModel : EntityGridViewModel<MaterialUsage>, ISelfTransientLifetime
{
    public MaterialUsageViewModel() 
    {
        InitializeToolBar();
    }

    public MaterialUsageViewModel(IDatabase database) : base(database) 
    {
        InitializeToolBar();
    }

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
        return connection.GetQuery<CalculationMaterial>()
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

    private void InitializeToolBar()
    {
        ToolBarItems.AddButtons(this,
            new ToolBarButtonModel("Изделие", "goods") { Command = OpenGoods },
            new ToolBarButtonModel("Калькуляция", "calculation") { Command = OpenCalculation },
            new ToolBarSeparatorModel(),
            new ToolBarButtonComboModel("Печать", "print"),
            new ToolBarButtonModel("Настройки", "settings"));
    }
}
