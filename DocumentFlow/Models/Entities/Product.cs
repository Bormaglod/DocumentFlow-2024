//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentFlow.Models.Entities;

public partial class Product : Directory
{
    /// <summary>
    /// Возвращает или устанавливает цену материала / изделия (без учёта НДС).
    /// </summary>
    [ObservableProperty]
    private decimal price;

    /// <summary>
    /// Возвращает или устанавливает процентную ставку НДС.
    /// </summary>
    [ObservableProperty]
    private int vat;

    /// <summary>
    /// Возвращает или устанавливает идектификатор единицы измерения.
    /// </summary>
    [ObservableProperty]
    private Measurement? measurement;

    /// <summary>
    /// Возвращает или устанавливает вес материала / изделия в граммах.
    /// </summary>
    [ObservableProperty]
    private decimal? weight;

    /// <summary>
    /// Возвращает или устанавливает наименование материала или изделия, которое будет использоваться в документах.
    /// </summary>
    [ObservableProperty]
    private string? docName;

    /// <summary>
    /// Возвращает список документов/файлов прикреплённых к данному материалу или изделию.
    /// </summary>
    [ObservableProperty]
    private IReadOnlyList<DocumentRefs>? thumbnails;

    /// <summary>
    /// Возвращает остаток материала или изделия на текущий момент.
    /// </summary>
    public decimal? ProductBalance { get; protected set; }
}
