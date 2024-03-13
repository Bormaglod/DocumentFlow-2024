//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public partial class ContractorViewModel : DirectoryEditorViewModel<Contractor>, ISelfTransientLifetime
{
    private readonly IOkopfRepository okopfRepository = null!;
    private readonly IAccountRepository accountRepository = null!;
    private readonly IPersonRepository personRepository = null!;

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

    public ContractorViewModel() { }

    public ContractorViewModel(IOkopfRepository okopfRepository, IAccountRepository accountRepository, IPersonRepository personRepository) : base()
    {
        this.okopfRepository = okopfRepository;
        this.accountRepository = accountRepository;
        this.personRepository = personRepository;
    }

    protected override string GetStandardHeader() => "Контрагент";

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<Contractor>(x => Okopf)
            .MappingQuery<Contractor>(x => Account)
            .MappingQuery<Contractor>(x => Person);
    }


    protected override void Load(IDbConnection connection)
    {
        Load<Okopf, Account, Person>(connection, (contractor, okopf, account, person) => 
        { 
            contractor.Okopf = okopf; 
            contractor.Account = account;
            contractor.Person = person;
            return contractor; 
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, Contractor? contractor)
    {
        base.InitializeEntityCollections(connection, contractor);
        Okopfs = okopfRepository.GetSlim(connection);
        if (Owner != null)
        {
            Accounts = accountRepository.GetSlim(connection, Owner);
        }

        Persons = personRepository.GetSlim(connection);
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