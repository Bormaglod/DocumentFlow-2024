//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using Dapper;

using DocumentFlow.Common.Collections;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public partial class OperationViewModel : DirectoryEditorViewModel<Operation>, ISelfTransientLifetime
{
    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private OperationType? operationType;

    [ObservableProperty]
    private int produced;

    [ObservableProperty]
    private int prodTime;

    [ObservableProperty]
    private int productionRate;

    [ObservableProperty]
    private DateTime? dateNorm;

    [ObservableProperty]
    private decimal salary;

    [ObservableProperty]
    private IList<OperationGoods>? operationGoods;

    [ObservableProperty]
    private OperationGoods? operationGoodsSelected;

    protected override string GetStandardHeader() => "Операция";

    protected override void RaiseAfterLoadDocument(Operation entity)
    {
        Code = entity.Code;
        ParentId = entity.ParentId;
        ItemName = entity.ItemName;
        OperationType = entity.OperationType;
        Produced = entity.Produced;
        ProdTime = entity.ProdTime;
        ProductionRate = entity.ProductionRate;
        DateNorm = entity.DateNorm;
        Salary = entity.Salary;
    }

    protected override void UpdateEntity(Operation entity)
    {
        entity.Code = Code;
        entity.ParentId = ParentId;
        entity.ItemName = ItemName;
        entity.OperationType = OperationType;
        entity.Produced = Produced;
        entity.ProdTime = ProdTime;
        entity.ProductionRate = ProductionRate;
        entity.DateNorm = DateNorm;
        entity.Salary = Salary;
    }

    protected override void Load() => Load<OperationType>((operation, type) => { operation.OperationType = type; return operation; });

    protected override void InitializeEntityCollections(IDbConnection connection, Operation? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        if (entity != null)
        {
            var sql = "select og.*, g.id, g.code, g.item_name from operation_goods as og left join goods g on g.id = og.goods_id where og.owner_id = :Id";
            var res = connection.Query<OperationGoods, Goods, OperationGoods>(sql, (og, goods) =>
            {
                og.Goods = goods;
                return og;
            }, entity);

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
