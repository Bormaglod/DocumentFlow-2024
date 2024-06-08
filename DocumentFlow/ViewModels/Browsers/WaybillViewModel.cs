//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public class WaybillViewModel<T> : DocumentViewModel<T>
    where T : Waybill
{
    public WaybillViewModel() { }

    public WaybillViewModel(IDatabase database, IConfiguration configuration, ILogger<WaybillViewModel<T>> logger) : base(database, configuration, logger) { }

    protected override Query MappingsQuery(Query query)
    {
        return base
            .MappingsQuery(query)
            .MappingQuery<T>(x => x.State)
            .MappingQuery<T>(x => x.Contractor)
            .MappingQuery<T>(x => x.Contract);
    }
}
