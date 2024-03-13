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

public partial class DeductionViewModel : DirectoryEditorViewModel<Deduction>, ISelfTransientLifetime
{
    private readonly IPersonRepository repoPerson = null!;

    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private BaseDeduction baseDeduction;

    [ObservableProperty]
    private Person? person;

    [ObservableProperty]
    private decimal value;

    [ObservableProperty]
    private IEnumerable<Person>? persons;

    public DeductionViewModel() { }

    public DeductionViewModel(IPersonRepository repoPerson)
    {
        this.repoPerson = repoPerson;
    }

    protected override string GetStandardHeader() => "Удержание";

    protected override void RaiseAfterLoadDocument(Deduction entity)
    {
        Code = entity.Code;
        ParentId = entity.ParentId;
        ItemName = entity.ItemName;
        BaseDeduction = entity.BaseDeduction;
        Person = entity.Person;
        Value = entity.Value;
    }

    protected override void UpdateEntity(Deduction entity)
    {
        entity.Code = Code;
        entity.ParentId = ParentId;
        entity.ItemName = ItemName;
        entity.BaseDeduction = BaseDeduction;
        entity.Person = Person;
        entity.Value = Value;
    }

    protected override void InitializeEntityCollections(IDbConnection connection, Deduction? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);
        Persons = repoPerson.GetSlim(connection);
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<Deduction>(x => x.Person);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<Person>(connection, (ded, person) =>
        {
            ded.Person = person;
            return ded;
        });
    }

    partial void OnCodeChanged(string value)
    {
        UpdateHeader(value);
    }
}
