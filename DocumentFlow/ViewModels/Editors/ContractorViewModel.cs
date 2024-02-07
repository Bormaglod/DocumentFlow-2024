//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public partial class ContractorViewModel : DirectoryEditorViewModel<Contractor>, ISelfTransientLifetime
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
    private SubjectsCivilLow? subjectsCivil;

    [ObservableProperty]
    private Person? person;

    [ObservableProperty]
    private IEnumerable<Okopf>? okopfs;

    [ObservableProperty]
    private IEnumerable<Account>? accounts;

    [ObservableProperty]
    private IEnumerable<Person>? persons;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    public ContractorViewModel() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

    public ContractorViewModel(IDatabase database) : base(database) 
    {
    }

    protected override string GetStandardHeader() => "Контрагент";

    protected override void Load()
    {
        Load<Okopf, Account, Person>((contractor, okopf, account, person) => 
        { 
            contractor.Okopf = okopf; 
            contractor.Account = account;
            contractor.Person = person;
            return contractor; 
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, Contractor? contractor)
    {
        Okopfs = GetForeignData<Okopf>(connection);
        Accounts = GetForeignData<Account>(connection, Id);        
        Persons = GetForeignDirectory<Person>(connection);
    }

    protected override void RaiseAfterLoadDocument(Contractor entity)
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
        SubjectsCivil = entity.SubjectCivilLow;
        Person = entity.Person;
    }

    protected override void UpdateEntity(Contractor entity)
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
        entity.SubjectCivilLow = SubjectsCivil;
        entity.Person = Person;
    }

    partial void OnCodeChanged(string value)
    {
        UpdateHeader(value ?? "?");
    }
}