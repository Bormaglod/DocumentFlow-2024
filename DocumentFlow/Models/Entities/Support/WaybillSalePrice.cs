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

[ProductContent(ProductContent.All)]
public partial class WaybillSalePrice : ProductPrice, IDiscriminator, IProductionLotSupport
{
    [ObservableProperty]
    [property: ForeignKey(FieldKey = "lot_id")]
    private ProductionLot? lot;

    [Write(false)]
    public string? Discriminator { get; set; }
}
