//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using Humanizer;

using SqlKata.Execution;

namespace DocumentFlow.Repository;

public class StatesRepository : IStatesRepository, ITransientLifetime
{
    private readonly IDatabase database;
    public StatesRepository(IDatabase database)
    {
        this.database = database;
    }

    public IReadOnlyList<State> GetStateTargets(BaseDocument document)
    {
        if (document.State != null)
        {
            return GetStateTargets(document.GetType(), document.State);
        }

        return Array.Empty<State>();
    }

    public IReadOnlyList<State> GetStateTargets(Type documentType, State fromState)
    {
        using var conn = database.OpenConnection();

        QueryParameters parameters = new()
        {
            Alias = "cs",
            IncludeDocumentsInfo = false,
            Quantity = QuantityInformation.None
        };

        return conn.GetQuery<ChangingState>(parameters)
            .Select("s.*")
            .Join("schema_states as ss", "ss.id", "cs.schema_id")
            .Join("document_type as dt", "dt.id", "ss.document_type_id")
            .Join("state as s", "s.id", "cs.to_state_id")
            .Where("dt.code", documentType.Name.Underscore())
            .Where("cs.from_state_id", fromState.Id)
            .Union(q => q
                .From("state")
                .Select("*")
                .Where("id", fromState.Id)
            )
            .Get<State>()
            .ToList();
    }
}
