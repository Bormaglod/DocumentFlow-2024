//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class OperationUsage : DocumentInfo
{
    [ObservableProperty]
    [property: ForeignKey(FieldKey = "owner_id")]
    public Calculation? calculation;

    [ObservableProperty]
    [property: ForeignKey(FieldKey = "owner_id", Table = "calculation")]
    public Goods? goods;

    [ObservableProperty]
    private decimal amount;
}
