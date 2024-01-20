//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Editors;

public partial class PersonViewModel : DirectoryEditorViewModel<Person>, ISelfTransientLifetime
{
    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? surname;

    [ObservableProperty]
    private string? firstName;

    [ObservableProperty]
    private string? middleName;

    [ObservableProperty]
    private string? phone;

    [ObservableProperty]
    private string? email;

    public PersonViewModel() { }

    public PersonViewModel(IDatabase database) : base(database) { }

    protected override string GetStandardHeader() => "Физ. лица";

    protected override void RaiseAfterLoadDocument(Person entity)
    {
        ParentId = entity.ParentId;
        Name = entity.ItemName;
        Surname = entity.Surname;
        FirstName = entity.FirstName;
        MiddleName = entity.MiddleName;
        Phone = entity.Phone;
        Email = entity.Email;
    }

    protected override void UpdateEntity(Person entity)
    {
        entity.ParentId = ParentId;
        entity.ItemName = Name;
        entity.Surname = Surname;
        entity.FirstName = FirstName;
        entity.MiddleName = MiddleName;
        entity.Phone = Phone;
        entity.Email = Email;
    }

    partial void OnNameChanged(string? value)
    {
        UpdateHeader(value ?? "?");
    }
}