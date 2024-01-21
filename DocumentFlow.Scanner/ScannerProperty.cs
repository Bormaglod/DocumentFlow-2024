//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using WIA;

namespace DocumentFlow.Scanner;

internal class ScannerProperty
{
    public required string Name { get; init; }
    public required int Id { get; init; }
    public required string Value { get; init; }
    public int SubTypeMin { get; set; }
    public int SubTypeMax { get; set; }
    public int SubTypeStep { get; set; }
    public Vector? SubTypeValues { get; set; }
    public WiaSubType SubType {  get; set; }
}
