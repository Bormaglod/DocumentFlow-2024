//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class MaterialUsage : DocumentInfo
{
    [DenyWriting]
    public Calculation? Calculation { get; set; }

    [DenyWriting]
    public Goods? Goods { get; set; }

    [DenyWriting]
    public decimal Amount { get; set; }
}
