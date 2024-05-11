//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models.Entities;

public class WaybillSale : Waybill
{
    public DateOnly? PaymentDate { get; protected set; }

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

    /*[WritableCollection]
    public IList<WaybillSalePrice> Prices { get; protected set; } = null!;*/
}
