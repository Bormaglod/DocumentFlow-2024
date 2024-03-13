//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public abstract partial class AccountingDocument : BaseDocument
{
    [ObservableProperty]
    [property: DenyWriting]
    private bool carriedOut;

    [ObservableProperty]
    [property: DenyWriting]
    private bool reCarriedOut;
}
