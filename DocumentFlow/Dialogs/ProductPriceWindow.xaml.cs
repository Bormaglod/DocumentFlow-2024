//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Data.Models;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для ProductPriceWindow.xaml
/// </summary>
public partial class ProductPriceWindow : Window, INotifyPropertyChanged
{
    private Product? selectedProduct;
    private decimal price;
    private IEnumerable<Product>? itemsSource;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ProductPriceWindow()
    {
        InitializeComponent();

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        string sql = @"
            select 
                p.id, p.code, p.item_name, p.is_folder, p.parent_id, m.id, m.abbreviation 
            from product as p 
                left join measurement as m on m.id = p.measurement_id 
            order by p.item_name";

        ItemsSource = conn.Query<Product, Measurement, Product>(
            sql,
            map: (product, measurement) =>
            {
                product.Measurement = measurement;
                return product;
            });
    }

    public IEnumerable<Product>? ItemsSource 
    { 
        get => itemsSource; 
        set
        {
            if (itemsSource != value) 
            { 
                itemsSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemsSource)));
            }
        }
    }

    public Product? SelectedProduct 
    { 
        get => selectedProduct; 
        set
        {
            if (selectedProduct != value)
            {
                selectedProduct = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProduct)));
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
            }
        }
    }

    public bool Create([MaybeNullWhen(false)] out PriceApproval row)
    {
        if (ShowDialog() == true)
        {
            row = new();
            if (SelectedProduct != null)
            {
                row.Product = SelectedProduct;
                row.Price = Price;
            }

            return true;
        }

        row = default;
        return false;
    }

    public bool Create(PriceApproval source, [MaybeNullWhen(false)] out PriceApproval row)
    {
        SelectedProduct = source.Product;
        Price = source.Price;

        return Create(out row);
    }

    public bool Edit(PriceApproval row)
    {
        SelectedProduct = row.Product;
        Price = row.Price;

        if (ShowDialog() == true)
        {
            row.Product = SelectedProduct;
            row.Price = Price;

            return true;
        }

        return false;
    }

    private void AcceptClick(object sender, RoutedEventArgs e)
    {
        if (SelectedProduct == null)
        {
            MessageBox.Show("Необходимо выбрать элемент справочника.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            DialogResult = true;
        }
    }
}
