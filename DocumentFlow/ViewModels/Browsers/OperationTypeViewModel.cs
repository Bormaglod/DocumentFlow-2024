﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class OperationTypeViewModel : DirectoryViewModel<OperationType>, ISelfTransientLifetime
{
    public OperationTypeViewModel() { }

    public OperationTypeViewModel(IDatabase database, IConfiguration configuration, ILogger<OperationTypeViewModel> logger) : base(database, configuration, logger) { }

    public override Type? GetEditorViewType() => typeof(Views.Editors.OperationTypeView);

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.ItemName))
        {
            columnInfo.State = ColumnVisibleState.AlwaysVisible;
        }
    }
}
