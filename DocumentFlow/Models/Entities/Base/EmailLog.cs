//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models.Entities;

public class EmailLog : ObservableObject
{
    [Key]
    public long Id { get; set; }

    public long EmailId { get; set; }

    public DateTime DateTimeSending { get; set; }

    public string ToAddress { get; set; } = string.Empty;
    
    public Guid ContractorId { get; set; }

    public string ContractorName { get; protected set; } = string.Empty;

    public Guid? ContractorGroupId { get; protected set; }

    public string? ContractorGroup { get; protected set; }

    public Guid? DocumentId { get; set; }

    public string? Code { get; protected set; }

    public string? DocumentName { get; protected set; }

    public DateTime DocumentDate { get; protected set; }

    public int DocumentNumber { get; protected set; }

    public string Document => $"{DocumentName} № {DocumentNumber} от {DocumentDate:d}";
}
