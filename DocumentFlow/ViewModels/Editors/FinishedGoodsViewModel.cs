//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;

using Syncfusion.Windows.Shared;

using System.Data;
using System.Windows.Input;

namespace DocumentFlow.ViewModels.Editors;

public partial class FinishedGoodsViewModel : DocumentEditorViewModel<FinishedGoods>, ISelfTransientLifetime
{
    private readonly IProductionLotRepository productionLotRepository = null!;
    private readonly IGoodsRepository goodsRepository = null!;

    [ObservableProperty]
    private ProductionLot? lot;

    [ObservableProperty]
    private IEnumerable<ProductionLot>? lots;

    [ObservableProperty]
    private Goods? product;

    [ObservableProperty]
    private IEnumerable<Goods>? products;

    [ObservableProperty]
    private decimal quantity;

    [ObservableProperty]
    private decimal? price;

    [ObservableProperty]
    private decimal? productCost;

    public FinishedGoodsViewModel(
        IProductionLotRepository productionLotRepository,
        IGoodsRepository goodsRepository,
        IOrganizationRepository organizationRepository) : base(organizationRepository)
    {
        this.productionLotRepository = productionLotRepository;
        this.goodsRepository = goodsRepository;
    }

    #region LotSelectedCommand

    private ICommand? lotSelectedCommand;

    public ICommand LotSelectedCommand
    {
        get
        {
            lotSelectedCommand ??= new DelegateCommand(OnLotSelectedCommand);
            return lotSelectedCommand;
        }
    }

    private void OnLotSelectedCommand(object parameter)
    {
        if (Lot?.Calculation != null)
        {
            Price = Lot?.Calculation.CostPrice;
        }
    }

    #endregion

    #region ProductSelectedCommand

    private ICommand? productSelectedCommand;

    public ICommand ProductSelectedCommand
    {
        get
        {
            productSelectedCommand ??= new DelegateCommand(OnProductSelectedCommand);
            return productSelectedCommand;
        }
    }

    private void OnProductSelectedCommand(object parameter)
    {
        if (Product != null)
        {
            var calc = goodsRepository.GetCurrentCalculation(Product);
            if (calc != null) 
            {
                Price = calc.CostPrice;
            }
        }
    }

    #endregion

    protected override string GetStandardHeader() => "Готовая продукция";

    protected override void RaiseAfterLoadDocument(FinishedGoods entity)
    {
        DocumentNumber = entity.DocumentNumber;
        DocumentDate = entity.DocumentDate;
        Organization = entity.Organization;
        Lot = entity.Lot;
        Quantity = entity.Quantity;
        Price = entity.Price;
        ProductCost = entity.ProductCost;
        Product = entity.Goods ?? (Lot?.Calculation?.Goods);
    }

    protected override void UpdateEntity(FinishedGoods entity)
    {
        entity.DocumentNumber = DocumentNumber;
        entity.DocumentDate = DocumentDate;
        entity.Organization = Organization;
        entity.Lot = Lot;
        entity.Quantity = Quantity;
        entity.Price = Price;
        entity.ProductCost = ProductCost;
        entity.Goods = Lot == null ? Product : Lot.Calculation?.Goods;
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<PurchaseRequest>(x => x.Organization)
            .MappingQuery<FinishedGoods>(x => x.Goods)
            .MappingQuery<Goods>(x => x.Measurement)
            .MappingQuery<FinishedGoods>(x => x.Lot)
            .MappingQuery<ProductionLot>(x => x.Calculation)
            .MappingQuery<Calculation>(x => x.Goods);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<Organization, Goods, Measurement, ProductionLot, Calculation, Goods>(connection,
            (fg, org, goods, measurement, lot, calc, calcGoods) =>
            {
                goods.Measurement = measurement;
                calc.Goods = calcGoods;
                lot.Calculation = calc;
                fg.Organization = org;
                fg.Lot = lot;
                fg.Goods = goods;

                return fg;
            });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, FinishedGoods? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);
        
        Lots = productionLotRepository.GetActive(connection, entity?.Lot);
        Products = goodsRepository.GetProducts(withMeasurements: true);
    }

    partial void OnLotChanged(ProductionLot? value)
    {
        Owner = value;

        if (value != null)
        {
            Product = value.Calculation?.Goods;
        }
    }

    partial void OnQuantityChanged(decimal value)
    {
        ProductCost = value * Price;
    }

    partial void OnPriceChanged(decimal? value)
    {
        ProductCost = value == null ? null : value * Quantity;
    }
}
