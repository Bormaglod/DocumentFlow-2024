//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models.Entities;

public class Email : ObservableObject
{
    [Key]
    public long Id { get; set; }
    public string? Address { get; set; }
    public string? MailHost { get; set; }
    public short MailPort { get; set; }
    public string? UserPassword { get; set; }
    public string? SignaturePlain { get; set; }
    public string? SignatureHtml { get; set; }
}
