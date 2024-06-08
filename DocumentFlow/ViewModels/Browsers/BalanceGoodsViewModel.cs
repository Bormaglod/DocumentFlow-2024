//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class BalanceGoodsViewModel : BalanceProductViewModel<BalanceGoods>, ISelfTransientLifetime
{
    public BalanceGoodsViewModel() { }

    public BalanceGoodsViewModel(IDatabase database, IConfiguration configuration, ILogger<BalanceGoodsViewModel> logger) : base(database, configuration, logger) { }
}
