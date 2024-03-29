//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;

using SqlKata;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class PurchaseRequestViewModel : DocumentViewModel<PurchaseRequest>, ISelfTransientLifetime
{
    public PurchaseRequestViewModel() { }

    public PurchaseRequestViewModel(IDatabase database, IConfiguration configuration) : base(database, configuration) { }
    //public override Type? GetEditorViewType() => typeof(Views.Editors.WireView);

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<PurchaseRequest>(x => x.State)
            .MappingQuery<PurchaseRequest>(x => x.Contractor)
            .MappingQuery<PurchaseRequest>(x => x.Contract);
    }

    protected override IReadOnlyList<PurchaseRequest> GetData(IDbConnection connection, Guid? id = null)
    {
        return DefaultQuery(connection, id, new QueryParameters() { Table = "purchase_request_ext" })
            .Get<PurchaseRequest, State, Contractor, Contract>(
                (pr, state, contractor, contract) =>
                {
                    pr.State = state;
                    pr.Contractor = contractor;
                    pr.Contract = contract;
                    return pr;
                })
            .ToList();
    }
}
