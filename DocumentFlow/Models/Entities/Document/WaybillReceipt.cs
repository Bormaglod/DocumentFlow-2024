//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class WaybillReceipt : Waybill
{
    [ObservableProperty]
    [property: DenyWriting]
    [property: ForeignKey(FieldKey = "owner_id")]
    public PurchaseRequest? purchaseRequest;

    public int? PurchaseRequestNumber { get; protected set; }
    public DateTime? PurchaseRequestDate { get; protected set; }
    public bool? PaymentExists
    {
        get
        {
            if (Paid == 0)
            {
                return false;
            }

            if (Paid == FullCost)
            {
                return true;
            }

            return null;
        }
    }
}
