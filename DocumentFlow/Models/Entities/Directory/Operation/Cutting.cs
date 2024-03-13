//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class Cutting : Operation
{
    /// <summary>
    /// Возвращает или устанавливает длину провода.
    /// </summary>
    [ObservableProperty]
    private int segmentLength;

    /// <summary>
    /// Возвращает или устанавливает длину зачистки с начала провода.
    /// </summary>
    [ObservableProperty]
    private decimal leftCleaning;

    /// <summary>
    /// Возвращает или устанавливает ширинк окна на которое снимается изоляция в начале провода.
    /// </summary>
    [ObservableProperty]
    private int leftSweep;

    /// <summary>
    /// Возвращает или устанавливает длину зачистки с конца провода.
    /// </summary>
    [ObservableProperty]
    private decimal rightCleaning;

    /// <summary>
    /// Возвращает или устанавливает ширину окна на которое снимается изоляция в конце провода.
    /// </summary>
    [ObservableProperty]
    private int rightSweep;

    /// <summary>
    /// Возвращает или устанавливает номер используемой программы.
    /// </summary>
    [ObservableProperty]
    [property: DenyCopying]
    private int? programNumber;
}
