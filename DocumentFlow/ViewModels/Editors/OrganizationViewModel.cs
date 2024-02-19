//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using Dapper;

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using SqlKata;

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

    protected override string GetStandardHeader() => "Организация";


    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<Organization>(x => x.Okopf)
            .MappingQuery<Organization>(x => x.Account);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<Okopf, Account>(connection, (org, okopf, account) => 
        { 
            org.Okopf = okopf; 
            org.Account = account;
            return org; 
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, Organization? org)
    {
        base.InitializeEntityCollections(connection, org);
        Okopfs = GetForeignData<Okopf>(connection);
        Accounts = GetForeignData<Account>(connection, Id);
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