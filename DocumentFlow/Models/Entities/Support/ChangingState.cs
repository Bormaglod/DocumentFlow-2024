//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models.Entities;

public class ChangingState
{
    [Key]
    public int Id { get; set; }
    public string NameChange { get; set; } = string.Empty;
    public Guid SchemaId { get; set; }
    public short FromStateId {  get; set; }
    public short ToStateId { get; set; }

    public override string ToString() => NameChange;
}
