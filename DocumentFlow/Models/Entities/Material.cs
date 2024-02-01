//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;

using Humanizer;

namespace DocumentFlow.Models.Entities;

public partial class Material : Product
{
    /// <summary>
    /// Возвращает или устанавливает минимальное количество материала возможное для заказа.
    /// </summary>
    [ObservableProperty]
    private decimal? minOrder;

    /// <summary>
    /// Возвращает или устанавливает альтернативный вариант артикула
    /// </summary>
    [ObservableProperty]
    private string? extArticle;

    /// <summary>
    /// Возвращает или устанавливает идентификатор типа провода (для записей с установленным типом 
    /// материала <see cref="MaterialKind.Wire"/> 
    /// </summary>
    [ObservableProperty]
    private Wire? wire;

    /// <summary>
    /// Возвращает материал являющийся оригиналом по отношению к текущему
    /// </summary>
    [ObservableProperty]
    [property: DenyWriting]
    private Material? cross;

    /// <summary>
    /// Возвращает или устанавливает тип материала в виде строки перечисления PostgresQL
    /// </summary>
    [EnumType("material_kind")]
    public string MaterialKind { get; set; } = "undefined";

    /// <summary>
    /// Возвращает true, если данный материал используется хотя бы в одной калькуляции (калькуляция
    /// должна быть в утверждённом состоянии).
    /// </summary>
    public bool MaterialUsing { get; protected set; }

    /// <summary>
    /// Возвращает код статуса цены материала. Возможные варианты: 0 - действующая цена, 1 - цены установлена
    /// вручную, 2 - цена является устаревшей.
    /// </summary>
    public int PriceStatus { get; protected set; }

    [DenyWriting]
    public MaterialKind Kind
    {
        get { return Enum.Parse<MaterialKind>(MaterialKind.Pascalize()); }
        set { MaterialKind = value.ToString().Underscore(); }
    }

    partial void OnCrossChanged(Material? value)
    {
        OwnerId = value?.Id;
    }
}
