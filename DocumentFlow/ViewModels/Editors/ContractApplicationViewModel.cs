//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using Dapper;

using DocumentFlow.Common.Collections;
using DocumentFlow.Data.Models;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.Data;
using System.Windows.Input;
using System.Windows;
using Syncfusion.Windows.Shared;
using DocumentFlow.Common;
using System.Security.Principal;

namespace DocumentFlow.ViewModels.Editors;

public partial class ContractApplicationViewModel : DirectoryEditorViewModel<ContractApplication>, ISelfTransientLifetime
{
    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private DateTime documentDate;

    [ObservableProperty]
    private DateTime dateStart;

    [ObservableProperty]
    private DateTime? dateEnd;

    [ObservableProperty]
    private string? note;

    [ObservableProperty]
    private IList<PriceApproval>? products;

    [ObservableProperty]
    private PriceApproval? productSelected;

    #region Commands

    #region AddProduct

    private ICommand? addProduct;

    public ICommand AddProduct
    {
        get
        {
            addProduct ??= new DelegateCommand(OnAddProduct);
            return addProduct;
        }
    }

    private void OnAddProduct(object parameter)
    {
        if (Products == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new ProductPriceWindow();
        if (dialog.Create(out var product))
        {
            Products.Add(product);
        }
    }

    #endregion

    #region EditProduct

    private ICommand? editProduct;

    public ICommand EditProduct
    {
        get
        {
            editProduct ??= new DelegateCommand(OnEditProduct);
            return editProduct;
        }
    }

    private void OnEditProduct(object parameter)
    {
        if (Products == null || ProductSelected == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new ProductPriceWindow();
        dialog.Edit(ProductSelected);
    }

    #endregion

    #region DeleteProduct

    private ICommand? deleteProduct;

    public ICommand DeleteProduct
    {
        get
        {
            deleteProduct ??= new DelegateCommand(OnDeleteProduct);
            return deleteProduct;
        }
    }

    private void OnDeleteProduct(object parameter)
    {
        if (Products != null && ProductSelected != null)
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            Products.Remove(ProductSelected);
        }
    }

    #endregion

    #region CopyProduct

    private ICommand? copyProduct;

    public ICommand CopyProduct
    {
        get
        {
            copyProduct ??= new DelegateCommand(OnCopyProduct);
            return copyProduct;
        }
    }

    private void OnCopyProduct(object parameter)
    {
        if (Products == null || ProductSelected == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new ProductPriceWindow();
        if (dialog.Create(ProductSelected, out var product))
        {
            Products.Add(product);
        }
    }

    #endregion

    #endregion

    protected override string GetStandardHeader() => "Приложение";

    protected override void RaiseAfterLoadDocument(ContractApplication entity)
    {
        base.RaiseAfterLoadDocument(entity);
        Code = entity.Code;
        ItemName = entity.ItemName;
        DocumentDate = entity.DocumentDate;
        DateStart = entity.DateStart;
        DateEnd = entity.DateEnd;
        Note = entity.Note;
    }

    protected override void UpdateEntity(ContractApplication entity)
    {
        entity.Code = Code;
        entity.ItemName = ItemName;
        entity.DocumentDate = DocumentDate;
        entity.DateStart = DateStart;
        entity.DateEnd = DateEnd;
        entity.Note = Note;
    }

    protected override void InitializeEntityCollections(IDbConnection connection, ContractApplication? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);
        if (entity != null)
        {
            var prices = new List<PriceApproval>();

            var sql = @"
                select 
                    pa.*, 
                    p.id, p.code, p.item_name, 
                    p.tableoid::regclass::varchar as product_table_name, 
                    m.id, m.abbreviation 
                from price_approval as pa 
                    left join product p on p.id = pa.product_id 
                    left join measurement m on m.id = p.measurement_id 
                where pa.owner_id = :Id";

            using var reader = connection.ExecuteReader(sql, entity);

            var priceParser = reader.GetRowParser<PriceApproval>(length: 4);
            var materialParser = reader.GetRowParser<Material>(startIndex: 4, length: 3);
            var goodsParser = reader.GetRowParser<Goods>(startIndex: 4, length: 3);
            var measurementParser = reader.GetRowParser<Measurement>(startIndex: 8, length: 2);

            while (reader.Read())
            {
                
                var priceApproval = priceParser(reader);

                var table = reader.GetString(reader.GetOrdinal("product_table_name"));
                Product product = table switch
                {
                    "material" => materialParser(reader),
                    "goods" => goodsParser(reader),
                    _ => throw new NotSupportedException(),
                };

                product.Measurement = measurementParser(reader);
                priceApproval.Product = product;
                prices.Add(priceApproval);
            }

            Products = new DependentCollection<PriceApproval>(entity, prices);
        }
    }

    partial void OnItemNameChanged(string? value)
    {
        UpdateHeader($"{value} №{Code} от {DocumentDate:d}");
    }

    partial void OnCodeChanged(string value)
    {
        UpdateHeader($"{ItemName} №{value} от {DocumentDate:d}");
    }

    partial void OnDocumentDateChanged(DateTime value)
    {
        UpdateHeader($"{ItemName} №{Code} от {value:d}");
    }
}
