//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Editors;

public partial class GoodsViewModel : ProductViewModel<Goods>, ISelfTransientLifetime
{
    [ObservableProperty]
    private bool isService;

    [ObservableProperty]
    private string? note;

    [ObservableProperty]
    private int? length;

    [ObservableProperty]
    private int? width;

    [ObservableProperty]
    private int? height;

    [ObservableProperty]
    private Calculation? calculation;

    public GoodsViewModel() { }

    public GoodsViewModel(IDatabase database) : base(database) { }

    protected override string GetStandardHeader() => "Изделие";

    protected override void RaiseAfterLoadDocument(Goods entity)
    {
        base.RaiseAfterLoadDocument(entity);
        IsService = entity.IsService;
        Note = entity.Note;
        Length = entity.Length;
        Width = entity.Width;
        Height = entity.Height;
        Calculation = entity.Calculation;
    }

    protected override void UpdateEntity(Goods entity)
    {
        entity.IsService = IsService;
        entity.Note = Note;
        entity.Length = Length;
        entity.Width = Width;
        entity.Height = Height;
        entity.Calculation = Calculation;
    }

    protected override void Load()
    {
        Load<Measurement, Calculation>((goods, measurement, calculation) =>
        {
            goods.Measurement = measurement;
            goods.Calculation = calculation;
            return goods;
        });
    }
}