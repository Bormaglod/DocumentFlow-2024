//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

namespace DocumentFlow.Messages;

public class OpenPdfReportMessage
{
    public required string PdfFile { get; init; }
    public required DocumentInfo Document { get; init; }
}
