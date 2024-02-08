//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;

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
        if (SelectedItem is MaterialUsage item) 
        {
            //WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(typeof(GoodsView), item.Goods));
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
        if (SelectedItem is MaterialUsage item)
        {
            //WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(typeof(GoodsView), item.Calculation));
        }
    }

    #endregion

    #endregion

    protected override IReadOnlyList<MaterialUsage> GetData(IDbConnection connection, Guid? id = null)
    {
        return GetQuery(connection)
            .From("calculation_material as cm")
            .Select("cm.{id, amount}")
            .Select("cm.item_id as owner_id")
            .Select("c.{id, code, item_name}")
            .Select("g.{id, code, item_name}")
            .Join("calculation as c", "c.id", "cm.owner_id")
            .Join("goods as g", "g.id", "c.owner_id")
            .WhereRaw("c.state = 'approved'::calculation_state")
            .Where("cm.item_id", Owner?.Id)
            .Get<MaterialUsage, Calculation, Goods>()
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
