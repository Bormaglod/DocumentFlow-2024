//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common;
using DocumentFlow.Common.Collections;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using SqlKata;

using Syncfusion.Windows.Shared;

using System.Data;
using System.Windows;
using System.Windows.Input;

namespace DocumentFlow.ViewModels.Editors;

public partial class OperationViewModel : BaseOperationViewModel<Operation>, ISelfTransientLifetime
{
    [ObservableProperty]
    private OperationType? operationType;

    [ObservableProperty]
    private IList<OperationGoods>? operationGoods;

    [ObservableProperty]
    private OperationGoods? operationGoodsSelected;

    [ObservableProperty]
    private IEnumerable<OperationType>? operationTypes;

    #region Commands

    #region AddOperationGoods

    private ICommand? addOperationGoods;

    public ICommand AddOperationGoods
    {
        get
        {
            addOperationGoods ??= new DelegateCommand(OnAddOperationGoods);
            return addOperationGoods;
        }
    }

    private void OnAddOperationGoods(object parameter)
    {
        if (OperationGoods == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new DirectoryItemWindow();
        if (dialog.Get(GetForeignDirectory<Goods>(conn), null, out var goods))
        {
            OperationGoods.Add(new OperationGoods() { Goods = goods });
        }
    }

    #endregion

    #region EditOperationGoods

    private ICommand? editOperationGoods;

    public ICommand EditOperationGoods
    {
        get
        {
            editOperationGoods ??= new DelegateCommand(OnEditOperationGoods);
            return editOperationGoods;
        }
    }

    private void OnEditOperationGoods(object parameter)
    {
        if (OperationGoods == null || OperationGoodsSelected == null || OperationGoodsSelected.Goods == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new DirectoryItemWindow();
        if (dialog.Get(GetForeignDirectory<Goods>(conn), OperationGoodsSelected.Goods, out var goods))
        {
            OperationGoodsSelected.Goods = goods;
        }
    }

    #endregion

    #region DeleteOperationGoods

    private ICommand? deleteOperationGoods;

    public ICommand DeleteOperationGoods
    {
        get
        {
            deleteOperationGoods ??= new DelegateCommand(OnDeleteOperationGoods);
            return deleteOperationGoods;
        }
    }

    private void OnDeleteOperationGoods(object parameter)
    {
        if (OperationGoods != null && OperationGoodsSelected != null)
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            OperationGoods.Remove(OperationGoodsSelected);
        }
    }

    #endregion

    #region CopyOperationGoods

    private ICommand? copyOperationGoods;

    public ICommand CopyOperationGoods
    {
        get
        {
            copyOperationGoods ??= new DelegateCommand(OnCopyOperationGoods);
            return copyOperationGoods;
        }
    }

    private void OnCopyOperationGoods(object parameter)
    {
        if (OperationGoods == null || OperationGoodsSelected == null || OperationGoodsSelected.Goods == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new DirectoryItemWindow();
        if (dialog.Get(GetForeignDirectory<Goods>(conn), OperationGoodsSelected.Goods, out var goods))
        {
            OperationGoods.Add(new OperationGoods() { Goods = goods });
        }
    }

    #endregion

    #endregion

    protected override string GetStandardHeader() => "Операция";

    protected override void RaiseAfterLoadDocument(Operation entity)
    {
        base.RaiseAfterLoadDocument(entity);
        OperationType = entity.OperationType;
    }

    protected override void UpdateEntity(Operation entity)
    {
        base.UpdateEntity(entity);
        entity.OperationType = OperationType;
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<Operation>(x => x.OperationType);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<OperationType>(connection, (operation, type) => 
        { 
            operation.OperationType = type; 
            return operation; 
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, Operation? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        OperationTypes = GetForeignData<OperationType>(connection);

        if (entity != null)
        {
            var res = connection.GetQuery<OperationGoods>(alias: "og")
                .Select("og.*")
                .MappingQuery<OperationGoods>(x => x.Goods, QuantityInformation.Directory)
                .Where("og.owner_id", entity.Id)
                .Get<OperationGoods, Goods>(
                    map: (og, goods) =>
                    {
                        og.Goods = goods;
                        return og;
                    }
                );

            OperationGoods = new DependentCollection<OperationGoods>(entity, res);
        }
    }

    protected override void UpdateDependents(IDbConnection connection, IDbTransaction? transaction = null)
    {
        if (OperationGoods != null)
        {
            connection.UpdateDependents(OperationGoods, transaction);
        }
    }
}
