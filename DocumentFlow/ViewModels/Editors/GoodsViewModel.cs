//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DocumentFlow.Common.Extensions;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public partial class GoodsViewModel : ProductViewModel<Goods>, ISelfTransientLifetime
{
    private readonly IGoodsRepository goodsRepository = null!;

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

    [ObservableProperty]
    private IEnumerable<Calculation>? calculations;

    public GoodsViewModel() { }

    public GoodsViewModel(IMeasurementRepository measurementRepository, IGoodsRepository goodsRepository) : base(measurementRepository)
    {
        this.goodsRepository = goodsRepository;
    }

    #region Commands

    [RelayCommand]
    private void ChangeGoodsCode()
    {
        var dialog = new CodeGeneratorWindow();
        if (dialog.Get(Code, out var code)) 
        {
            Code = code;
        }
    }

    #endregion

    public override DocumentInfo? GetReportingDocument(Report report) => Entity?.Calculation;

    protected override void RegisterReports()
    {
        base.RegisterReports();
        RegisterReport(Report.Specification);
        RegisterReport(Report.ProcessMap);
    }

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

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<Goods>(x => x.Measurement)
            .MappingQuery<Goods>(x => x.Calculation);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<Measurement, Calculation>(connection, (goods, measurement, calculation) =>
        {
            goods.Measurement = measurement;
            goods.Calculation = calculation;
            return goods;
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, Goods? goods = null)
    {
        base.InitializeEntityCollections(connection, goods);

        if (goods != null) 
        {
            Calculations = goodsRepository.GetCalculations(connection, goods);
        }
    }
}