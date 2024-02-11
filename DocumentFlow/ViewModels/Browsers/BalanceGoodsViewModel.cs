﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Browsers;

public class BalanceGoodsViewModel : BalanceProductViewModel<BalanceGoods>, ISelfTransientLifetime
{
    public BalanceGoodsViewModel() { }

    public BalanceGoodsViewModel(IDatabase database) : base(database) { }
}
