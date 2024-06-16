//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DocumentFlow.Common;
using DocumentFlow.Common.Collections;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Data;
using System.Windows;

namespace DocumentFlow.ViewModels.Editors;

public partial class OperationViewModel : BaseOperationViewModel<Operation>, ISelfTransientLifetime
{
    private readonly IGoodsRepository repoGoods = null!;
    private readonly IOperationTypeRepository repoOperationTypes = null!;
    private readonly IOperationRepository repoOperation = null!;

    [ObservableProperty]
    private OperationType? operationType;

    [ObservableProperty]
    private DependentCollection<OperationGoods>? operationGoods;

    [ObservableProperty]
    private OperationGoods? operationGoodsSelected;

    [ObservableProperty]
    private IEnumerable<OperationType>? operationTypes;

    public OperationViewModel() { }

    public OperationViewModel(IGoodsRepository repoGoods, IOperationTypeRepository repoOperationType, IOperationRepository repoOperation) : base()
    {
        this.repoGoods = repoGoods;
        this.repoOperationTypes = repoOperationType;
        this.repoOperation = repoOperation;
    }

    #region Commands

    [RelayCommand]
    private void AddOperationGoods()
    {
        if (OperationGoods == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new DirectoryItemWindow();
        if (dialog.Get(repoGoods.GetSlim(conn), null, out var product))
        {
            OperationGoods.Add(new OperationGoods() { Goods = product });
        }
    }

    [RelayCommand]
    private void EditOperationGoods()
    {
        if (OperationGoods == null || OperationGoodsSelected == null || OperationGoodsSelected.Goods == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new DirectoryItemWindow();
        if (dialog.Get(repoGoods.GetSlim(conn), OperationGoodsSelected.Goods, out var product))
        {
            OperationGoodsSelected.Goods = product;
        }
    }

    [RelayCommand]
    private void DeleteOperationGoods()
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

    [RelayCommand]
    private void CopyOperationGoods()
    {
        if (OperationGoods == null || OperationGoodsSelected == null || OperationGoodsSelected.Goods == null)
        {
            return;
        }

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        var dialog = new DirectoryItemWindow();
        if (dialog.Get(repoGoods.GetSlim(conn), OperationGoodsSelected.Goods, out var product))
        {
            OperationGoods.Add(new OperationGoods() { Goods = product });
        }
    }

    #endregion

    protected override string GetStandardHeader() => "Операция";

    protected override void DoAfterLoadDocument(Operation entity)
    {
        base.DoAfterLoadDocument(entity);
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

    protected override void InitializeEntityCollections(IDbConnection connection, Operation? operation = null)
    {
        base.InitializeEntityCollections(connection, operation);

        OperationTypes = repoOperationTypes.GetSlim(connection);
        
        if (operation != null)
        {
            OperationGoods = new DependentCollection<OperationGoods>(operation, repoOperation.GetGoods(connection, operation));
        }
        else
        {
            OperationGoods ??= [];
        }    
    }

    protected override void UpdateDependents(IDbConnection connection, Operation operation, IDbTransaction? transaction = null)
    {
        if (OperationGoods != null)
        {
            OperationGoods.Owner = operation;
            connection.UpdateDependents(OperationGoods, transaction);
        }
    }
}
