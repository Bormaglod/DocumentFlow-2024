//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using Dapper;

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public partial class OrganizationViewModel : DirectoryEditorViewModel<Organization>, ISelfTransientLifetime
{
    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private string? fullName;

    [ObservableProperty]
    private decimal? inn;

    [ObservableProperty]
    private decimal? kpp;

    [ObservableProperty]
    private decimal? okpo;

    [ObservableProperty]
    private decimal? ogrn;

    [ObservableProperty]
    private Okopf? okopf;

    [ObservableProperty]
    private Account? account;

    [ObservableProperty]
    private string? address;

    [ObservableProperty]
    private string? phone;

    [ObservableProperty]
    private string? email;

    [ObservableProperty]
    private bool defaultOrg;

    [ObservableProperty]
    private IEnumerable<Okopf>? okopfs;

    [ObservableProperty]
    private IEnumerable<Account>? accounts;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    public OrganizationViewModel() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

    public OrganizationViewModel(IDatabase database) : base(database) 
    {
    }

    protected override string GetStandardHeader() => "Организация";

    protected override void Load()
    {
        Load<Okopf, Account>((org, okopf, account) => 
        { 
            org.Okopf = okopf; 
            org.Account = account;
            return org; 
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, Organization? contractor)
    {
        Okopfs = connection.Query<Okopf>("select * from okopf where not deleted");
        Accounts = connection.Query<Account>("select * from account where not deleted and owner_id = :id", new { id = Id });
    }

    protected override void RaiseAfterLoadDocument(Organization entity)
    {
        ParentId = entity.ParentId;
        Code = entity.Code;
        ItemName = entity.ItemName;
        FullName = entity.FullName;
        Inn = entity.Inn;
        Kpp = entity.Kpp;
        Ogrn = entity.Ogrn;
        Okpo = entity.Okpo;
        Okopf = entity.Okopf;
        Account = entity.Account;
        Address = entity.Address;
        Phone = entity.Phone;
        Email = entity.Email;
        DefaultOrg = entity.DefaultOrg;
    }

    protected override void UpdateEntity(Organization entity)
    {
        entity.ParentId = ParentId;
        entity.Code = Code;
        entity.ItemName = ItemName;
        entity.FullName = FullName;
        entity.Inn = Inn;
        entity.Kpp = Kpp;
        entity.Ogrn = Ogrn;
        entity.Okpo = Okpo;
        entity.Okopf = Okopf;
        entity.Account = Account;
        entity.Address = Address;
        entity.Phone = Phone;
        entity.Email = Email;
        entity.DefaultOrg = DefaultOrg;
    }

    partial void OnCodeChanged(string value)
    {
        UpdateHeader(value ?? "?");
    }
}