﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class OperationViewModel : DirectoryViewModel<Operation>, ISelfTransientLifetime
{
    public OperationViewModel() { }

    public OperationViewModel(IDatabase database, IConfiguration configuration, ILogger<OperationViewModel> logger) : base(database, configuration, logger) { }

    public override Type? GetEditorViewType() => typeof(Views.Editors.OperationView);

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.ItemName))
        {
            columnInfo.State = ColumnVisibleState.AlwaysVisible;
        }
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .SelectRaw("exists(select 1 from calculation_operation co join calculation c on c.id = co.owner_id where co.item_id = t0.id and c.state = 'approved'::calculation_state) as operation_using")
            .MappingQuery<Operation>(x => x.OperationType);
    }

    protected override IReadOnlyList<Operation> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id, new QueryParameters() { FromOnly = true })
            .Get<Operation, OperationType>(
                map: (operation, type) =>
                {
                    operation.OperationType = type;
                    return operation;
                }
            )
            .ToList();
    }
}
