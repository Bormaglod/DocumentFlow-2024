//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class FinishedGoods : AccountingDocument
{
    [ObservableProperty]
    [property: DenyWriting]
    [property: ForeignKey(FieldKey = "owner_id")]
    private ProductionLot? lot;

    [ObservableProperty]
    private Goods? goods;

    [ObservableProperty]
    private decimal quantity;

    [ObservableProperty]
    private decimal? price;

    [ObservableProperty]
    private decimal? productCost;
}
