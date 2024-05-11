//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;

namespace DocumentFlow.Models.Entities;

[ProductContent(ProductContent.Goods)]
public partial class ProductionOrderPrice : ProductPrice, IProductCalculation
{
    [ObservableProperty]
    private Calculation? calculation;
}
