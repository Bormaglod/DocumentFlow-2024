//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public abstract partial class BaseDocument : DocumentInfo
{
    [ObservableProperty]
    [property: DenyCopying]
    private int? documentNumber;

    [ObservableProperty]
    [property: DenyCopying]
    private DateTime documentDate;

    [ObservableProperty]
    [property: DenyWriting]
    private Organization? organization;

    [ObservableProperty]
    [property: DenyCopying]
    [property: DenyWriting]
    private State? state;

    public override string ToString() => $"№{DocumentNumber} от {DocumentDate:d}";
}
