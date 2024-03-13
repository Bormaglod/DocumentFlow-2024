//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models.Entities;

public partial class CalculationOperationProperty : ObservableObject, IDependentEntity
{
    [ObservableProperty]
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    private Property property;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

    [ObservableProperty]
    private string? propertyValue;

    [Key]
    public long Id { get; set; }

    public Guid OperationId { get; set; }

    public void SetOwner(DocumentInfo owner) => OperationId = owner.Id;
}
