//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class Company : Directory
{
    [ObservableProperty]
    private string? fullName;

    [ObservableProperty]
    private decimal? inn;

    [ObservableProperty]
    private decimal? kpp;

    [ObservableProperty]
    private decimal? ogrn;

    [ObservableProperty]
    private decimal? okpo;

    [ObservableProperty]
    private Okopf? okopf;

    // https://github.com/CommunityToolkit/dotnet/issues/413
    [ObservableProperty]
    [property: DenyCopying]
    private Account? account;

    public bool ContainsContract(Contract contract) => contract.OwnerId == Id;
}
