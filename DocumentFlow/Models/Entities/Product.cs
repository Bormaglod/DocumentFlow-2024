//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public partial class Product : Directory
{
    /*private static readonly Dictionary<int, string> taxes = new()
    {
        [0] = "Без НДС",
        [10] = "10%",
        [20] = "20%"
    };*/

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

    /// <summary>
    /// Возвращает true, если материал или изделие имеют сохранённые эскизы изображений.
    /// </summary>
    public bool HasThumbnails { get; protected set; }

    //public static IReadOnlyDictionary<int, string> Taxes => taxes;

    protected override string GetRowStatusImageName()
    {
        if (HasThumbnails)
        {
            if (HasDocuments == true)
            {
                return "icons8-document-attached-images-16";
            }
            else
            {
                return "icons8-document-images-16";
            }
        }

        return base.GetRowStatusImageName();
    }
}
