﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Extensions;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;

using Syncfusion.Windows.Shared;

using System.Data;
using System.Windows.Input;

namespace DocumentFlow.ViewModels.Editors;

public partial class GoodsViewModel : ProductViewModel<Goods>, ISelfTransientLifetime
{
    private readonly ICalculationRepository calculationRepository = null!;

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

    public GoodsViewModel(IMeasurementRepository measurementRepository, ICalculationRepository calculationRepository) : base(measurementRepository)
    {
        this.calculationRepository = calculationRepository;
    }

    #region Commands

    #region CreateGroup

    private ICommand? changeGoodsCode;

    public ICommand ChangeGoodsCode
    {
        get
        {
            changeGoodsCode ??= new DelegateCommand(OnСhangeGoodsCode);
            return changeGoodsCode;
        }
    }

    private void OnСhangeGoodsCode(object parameter)
    {
        var dialog = new CodeGeneratorWindow();
        if (dialog.Get(Code, out var code)) 
        {
            Code = code;
        }
    }

    #endregion

    #endregion

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
            Calculations = calculationRepository.GetCalculations(connection, goods);
        }
    }
}