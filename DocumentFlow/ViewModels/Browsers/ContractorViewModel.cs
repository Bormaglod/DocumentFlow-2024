﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class ContractorViewModel : DirectoryViewModel<Contractor>, ISelfTransientLifetime
{
    public ContractorViewModel() { }

    public ContractorViewModel(IDatabase database, IConfiguration configuration, ILogger<ContractorViewModel> logger) : base(database, configuration, logger) { }

    public override Type? GetEditorViewType() => typeof(Views.Editors.ContractorView);

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.Code))
        {
            columnInfo.State = ColumnVisibleState.AlwaysVisible;
        }
    }

    protected override IReadOnlyList<Contractor> GetData(IDbConnection connection, Guid? id)
    {
        return DefaultQuery(connection, id)
            .MappingQuery<Contractor>(x => x.Okopf)
            .Get<Contractor, Okopf>(
                map: (contractor, okopf) => 
                { 
                    contractor.Okopf = okopf; 
                    return contractor; 
                })
            .ToList();
    }
}
