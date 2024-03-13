//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Data;

public class CodeGenerator
{
    [Key]
    public int Id { get; set; }
    public short CodeId { get; set; }
    public string CodeName { get; set; } = string.Empty;

    public override string ToString() => CodeName;
}
