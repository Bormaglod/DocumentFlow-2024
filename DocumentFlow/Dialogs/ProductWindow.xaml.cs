//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common;
using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для ProductWindow.xaml
/// </summary>
public partial class ProductWindow : Window, INotifyPropertyChanged
{
    private enum DialogStatus { None, Create, Edit };

    public event PropertyChangedEventHandler? PropertyChanged;

    private Product? product;
    private Calculation? calculation;
    private decimal amount;
    private decimal price;
    private decimal productCost;
    private int tax;
    private decimal taxValue;
    private decimal fullCost;

    private IEnumerable<Product>? products;
    private IEnumerable<Calculation>? calculations;
    private IEnumerable<int> taxes = new List<int>() { 0, 10, 20 };

    private ProductContent content;
    private bool withCalculation;
    private bool withLot;
    private bool includePrice;
    private bool isTaxPayer;

    private DialogStatus dialogStatus = DialogStatus.None;

    public ProductWindow()
    {
        InitializeComponent();
    }

    public Contract? Contract { get; set; }

    public IEnumerable<Product>? Products
    {
        get => products;
        set
        {
            if (products != value)
            {
                products = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Products)));
            }
        }
    }

    public IEnumerable<Calculation>? Calculations
    {
        get => calculations;
        set
        {
            if (calculations != value)
            {
                calculations = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Calculations)));
            }
        }
    }

    public IEnumerable<int> Taxes
    {
        get => taxes;
        set
        {
            if (taxes != value)
            {
                taxes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Taxes)));
            }
        }
    }

    public Product? Product
    {
        get => product;
        set
        {
            if (product != value)
            {
                product = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Product)));
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
                }
            }
        }
    }

    public Calculation? Calculation
    {
        get => calculation;
        set
        {
            if (calculation != value)
            {
                calculation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Calculation)));
            }
        }
    }

    public decimal Amount
    {
        get => amount;
        set
        {
            if (amount != value)
            {
                amount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount)));

                ProductCost = Price * value;
            }
        }
    }

    public decimal Price
    {
        get => price;
        set
        {
            if (price != value)
            {
                price = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Price)));

                ProductCost = Amount * value;
            }
        }
    }

    public decimal ProductCost
    {
        get => productCost;
        set
        {
            if (productCost != value)
            {
                productCost = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProductCost)));

                TaxValue = value * Tax / 100;
            }
        }
    }

    public int Tax
    {
        get => tax;
        set
        {
            if (tax != value)
            {
                tax = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tax)));

                TaxValue = ProductCost * value / 100;
            }
        }
    }

    public decimal TaxValue
    {
        get => taxValue;
        set
        {
            if (taxValue != value)
            {
                taxValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TaxValue)));

                FullCost = ProductCost + value;
            }
        }
    }

    public decimal FullCost
    {
        get => fullCost;
        set
        {
            if (fullCost != value)
            {
                fullCost = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullCost)));
            }
        }
    }

    public bool WithCalculation
    {
        get => withCalculation;
        set
        {
            if (withCalculation != value)
            {
                withCalculation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WithCalculation)));
            }
        }
    }

    public bool WithLot
    {
        get => withLot;
        set
        {
            if (withLot != value)
            {
                withLot = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WithLot)));
            }
        }
    }

    public bool IncludePrice
    {
        get => includePrice;
        set
        {
            if (includePrice != value)
            {
                includePrice = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IncludePrice)));
            }
        }
    }

    public bool IsTaxPayer
    {
        get => isTaxPayer;
        set
        {
            if (isTaxPayer != value)
            {
                isTaxPayer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(isTaxPayer)));
            }
        }
    }

    public bool Create<T>([MaybeNullWhen(false)] out T row) where T : ProductPrice, new()
    {
        dialogStatus = DialogStatus.Create;
        try
        {
            UpdateControls(typeof(T));

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

            if (product is IProductCalculation p && Calculation != null)
            {
                p.Calculation = Calculation;
            }

            /*if (product is IProductionLotSupport lot && selectLot.SelectedItem != Guid.Empty)
            {
                lot.LotId = selectLot.SelectedItem;
            }*/

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

            /*if (product is IProductionLotSupport lot && lot.LotId != null)
            {
                selectLot.DataSource = new ProductionLot[]
                {
                    services.GetRequiredService<IProductionLotRepository>().Get(lot.LotId.Value)
                };

                selectLot.SelectedItem = lot.LotId.Value;
            }*/

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

            /*if (product is IProductionLotSupport _lot)
            {
                _lot.LotId = selectLot.SelectedItem == Guid.Empty ? null : selectLot.SelectedItem;
            }*/

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
        IEnumerable<Material> materials = Array.Empty<Material>();
        IEnumerable<Goods> goods = Array.Empty<Goods>();

        if (content == ProductContent.Materials || content == ProductContent.All)
        {
            var materialsRepository = ServiceLocator.Context.GetService<IMaterialRepository>();
            materials = materialsRepository.GetSlim();
        }

        if (content == ProductContent.Goods || content == ProductContent.All)
        {
            var goodsRepository = ServiceLocator.Context.GetService<IGoodsRepository>();
            goods = goodsRepository.GetSlim();
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
}
