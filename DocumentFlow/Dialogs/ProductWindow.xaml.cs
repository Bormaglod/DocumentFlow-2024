﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common;
using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using Humanizer;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для ProductWindow.xaml
/// </summary>
[INotifyPropertyChanged]
public partial class ProductWindow : Window
{
    private enum DialogStatus { None, Create, Edit };

    private ProductContent content;
    private DialogStatus dialogStatus = DialogStatus.None;

    [ObservableProperty]
    private Product? product;

    [ObservableProperty]
    private Calculation? calculation;

    [ObservableProperty]
    private ProductionLot? lot;

    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private decimal price;

    [ObservableProperty]
    private decimal productCost;

    [ObservableProperty]
    private int tax;

    [ObservableProperty]
    private decimal taxValue;

    [ObservableProperty]
    private decimal fullCost;

    [ObservableProperty]
    private IEnumerable<Product>? products;

    [ObservableProperty]
    private IEnumerable<Calculation>? calculations;

    [ObservableProperty]
    private IEnumerable<ProductionLot>? lots;

    [ObservableProperty]
    private IEnumerable<int> taxes = [0, 10, 20];

    [ObservableProperty]
    private bool withCalculation;

    [ObservableProperty]
    private bool withLot;

    [ObservableProperty]
    private bool includePrice;

    [ObservableProperty]
    private bool isTaxPayer;

    [ObservableProperty]
    private bool isInvalidCalculation = false;

    public ProductWindow()
    {
        InitializeComponent();
    }

    public Contractor? Contractor { get; set; }
    public Contract? Contract { get; set; }

    public bool Create<T>([MaybeNullWhen(false)] out T row) where T : ProductPrice, new()
    {
        dialogStatus = DialogStatus.Create;
        try
        {
            UpdateControls(typeof(T));

            if (WithLot)
            {
                Lots = GetLots();
            }

            if (IsTaxPayer)
            {
                Tax = 20;
            }
        }
        finally
        {
            dialogStatus = DialogStatus.None;
        }

        selectProduct.Focus();

        if (ShowDialog() == true)
        {
            ArgumentNullException.ThrowIfNull(Product);

            row = new()
            {
                Product = Product,
                Amount = Amount,
                Price = Price,
                ProductCost = ProductCost,
                Tax = Tax,
                TaxValue = TaxValue,
                FullCost = FullCost
            };

            if (row is IProductCalculation p && Calculation != null)
            {
                p.Calculation = Calculation;
            }

            if (row is IProductionLotSupport lot && Lot != null)
            {
                lot.Lot = Lot;
            }

            if (row is IDiscriminator discriminator)
            {
                discriminator.Discriminator = Product.GetType().Name.Underscore();
            }

            return true;
        }

        row = default;
        return false;
    }

    public bool CreateFrom<T>(T product, [MaybeNullWhen(false)] out T row) where T : ProductPrice, new()
    {
        row = new()
        {
            Product = product.Product,
            Amount = product.Amount,
            Price = product.Price,
            ProductCost = product.ProductCost,
            Tax = product.Tax,
            TaxValue = product.TaxValue,
            FullCost = product.FullCost
        };

        if (Edit(row))
        {
            return true;
        }

        row = null;
        return false;
    }

    public bool Edit<T>(T product) where T : ProductPrice
    {
        IProductCalculation? prod = product as IProductCalculation;

        dialogStatus = DialogStatus.Edit;
        try
        {
            UpdateControls(typeof(T));

            if (product is IProductionLotSupport lot && lot.Lot != null)
            {
                Lots = GetLots(Lot);
                Lot = lot.Lot;
            }

            Product = product.Product;
            Amount = product.Amount;
            Price = product.Price;
            ProductCost = product.ProductCost;
            Tax = product.Tax;
            TaxValue = product.TaxValue;
            FullCost = product.FullCost;
            Calculation = prod?.Calculation;
        }
        finally
        {
            dialogStatus = DialogStatus.None;
        }

        textAmount.Focus();

        if (ShowDialog() == true)
        {
            ArgumentNullException.ThrowIfNull(Product);

            product.Product = Product;
            product.Amount = Amount;
            product.Price = Price;
            product.ProductCost = ProductCost;
            product.Tax = Tax;
            product.TaxValue = TaxValue;
            product.FullCost = FullCost;

            if (prod != null)
            {
                prod.Calculation = Calculation;
            }

            if (product is IProductionLotSupport _lot)
            {
                _lot.Lot = Lot;
            }

            if (product is IDiscriminator discriminator)
            {
                discriminator.Discriminator = Product.GetType().Name.Underscore();
            }

            return true;
        }

        return false;
    }

    private void UpdateControls(Type productType)
    {
        WithCalculation = productType.GetInterface(nameof(IProductCalculation)) != null;
        WithLot = productType.GetInterface(nameof(IProductionLotSupport)) != null;

        content = productType.GetCustomAttribute<ProductContentAttribute>()?.Content ?? throw new Exception($"Использование ProductPriceDialog с типом {productType.Name} возможно только при условии наличия у этого типа атрибута ProductContentAttribute");

        IncludePrice = productType.GetCustomAttribute<ProductExcludingPriceAttribute>() == null;
        IsTaxPayer = Contract != null && Contract.TaxPayer;

        Products = GetProducts();
    }

    private void UpdateCalculations()
    {
        if (Product is not Goods goods)
        {
            Calculations = null;
        }
        else
        {
            var goodsRepository = ServiceLocator.Context.GetService<IGoodsRepository>();
            Calculations = goodsRepository.GetCalculations(goods);
        }
    }

    private void UpdatePrice()
    {
        if (!IncludePrice)
        {
            return;
        }

        decimal avgPrice = 0;

        if (Product is Material material)
        {
            var repo = ServiceLocator.Context.GetService<IMaterialRepository>();
            if (repo != null)
            {
                avgPrice = repo.GetAveragePrice(material);
            }
        }
        else if (Product is Goods goods && Contract != null)
        {
            var repo = ServiceLocator.Context.GetService<IContractRepository>();
            avgPrice = repo.GetPrice(Contract, goods);
        }

        if (avgPrice == 0 && Product != null)
        {
            avgPrice = Product.Price;
        }

        Price = Math.Round(avgPrice, 2);
    }

    private IEnumerable<Product> GetProducts()
    {
        IEnumerable<Material> materials = [];
        IEnumerable<Goods> goods = [];

        if (content == ProductContent.Materials || content == ProductContent.All)
        {
            var materialsRepository = ServiceLocator.Context.GetService<IMaterialRepository>();
            materials = materialsRepository.GetProducts(withMeasurements: true);
        }

        if (content == ProductContent.Goods || content == ProductContent.All)
        {
            var goodsRepository = ServiceLocator.Context.GetService<IGoodsRepository>();
            goods = goodsRepository.GetProducts(withMeasurements: true);
        }

        return materials
            .OfType<Product>()
            .Union(goods)
            .OrderByDescending(x => x.IsFolder)
            .ThenBy(x => x.ItemName);
    }

    private void AcceptClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void ProductWindow_ContentRendered(object sender, EventArgs e)
    {
        //selectProduct.Focus();
    }

    private void UpdateTaxValue(decimal taxValue)
    {
        if (taxValue == TaxValue)
        {
            FullCost = ProductCost;
        }
        else
        {
            TaxValue = taxValue;
        }
    }

    private IEnumerable<ProductionLot> GetLots(ProductionLot? current = null)
    {
        var lots = ServiceLocator.Context.GetService<IProductionLotRepository>();
        if (Contractor == null && Product == null)
        {
            return lots.GetActive(current);
        }

        var goods = Product as Goods;

        if (Contractor != null && goods != null)
        {
            return lots.GetActive(Contractor, goods, current);
        }

        if (goods != null)
        {
            return lots.GetActive(goods, current);
        }

        if (Contractor != null)
        {
            return lots.GetActive(Contractor, current);
        }

        return [];
    }

    partial void OnProductChanged(Product? value)
    {
        UpdateCalculations();
        UpdatePrice();

        if (dialogStatus == DialogStatus.None)
        {
            if (WithCalculation)
            {
                selectCalculation.Focus();
            }
            else
            {
                textAmount.Focus();
            }

            if (WithLot)
            {
                Lots = GetLots(Lot);
            }
        }
    }

    partial void OnLotChanged(ProductionLot? value)
    {
        if (value == null || Product != null)
        {
            Lots = GetLots();
            return;
        }

        Product = value.Calculation?.Goods;
    }

    partial void OnCalculationChanged(Calculation? value)
    {
        IsInvalidCalculation = value == null || value.CalculationState != CalculationState.Approved;
    }

    partial void OnAmountChanged(decimal value)
    {
        ProductCost = Price * value;
    }

    partial void OnPriceChanged(decimal value)
    {
        ProductCost = Amount * value;
    }

    partial void OnProductCostChanged(decimal value)
    {
        UpdateTaxValue(value * Tax / 100);
    }

    partial void OnTaxChanged(int value)
    {
        UpdateTaxValue(ProductCost * value / 100);
    }

    partial void OnTaxValueChanged(decimal value)
    {
        FullCost = ProductCost + value;
    }
}
