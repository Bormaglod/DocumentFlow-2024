//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class BalanceMaterialViewModel : BalanceProductViewModel<BalanceMaterial>, ISelfTransientLifetime
{
    public BalanceMaterialViewModel() { }

    public BalanceMaterialViewModel(IDatabase database) : base(database) { }
}