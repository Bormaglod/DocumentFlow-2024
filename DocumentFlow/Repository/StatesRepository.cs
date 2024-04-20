//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Exceptions;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using Humanizer;

using SqlKata;
using SqlKata.Execution;

using Syncfusion.Data.Extensions;

using System.Collections.Concurrent;
using System.Data;

namespace DocumentFlow.Repository;

public class StatesRepository : IStatesRepository, ITransientLifetime
{
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IDictionary<short, IEnumerable<State>>> documentStates = new();

    private readonly IDatabase database;

    public StatesRepository(IDatabase database)
    {
        this.database = database;
    }

    public State Get(short id)
    {
        using var conn = database.OpenConnection();
        return Get(conn, id);
    }

    public State Get(IDbConnection connection, short id)
    {
        return connection.QuerySingle<State>("select * from state where id = :id", new { id });
    }

    public async Task<State> GetAsync(short id)
    {
        using var conn = database.OpenConnection();
        return await GetAsync(conn, id);
    }

    public async Task<State> GetAsync(IDbConnection connection, short id)
    {
        return await connection.QuerySingleAsync<State>("select * from state where id = :id", new { id });
    }

    public IReadOnlyList<State> GetStateTargets(BaseDocument document)
    {
        if (document.State != null)
        {
            return GetStateTargets(document.GetType(), document.State);
        }

        return Array.Empty<State>();
    }

    public IReadOnlyList<State> GetStateTargets(IDbConnection connection, BaseDocument document)
    {
        if (document.State != null)
        {
            return GetStateTargets(connection, document.GetType(), document.State);
        }

        return Array.Empty<State>();
    }

    public IReadOnlyList<State> GetStateTargets(Type documentType, State fromState)
    {
        using var conn = database.OpenConnection();
        return GetStateTargets(conn, documentType, fromState);
    }

    public IReadOnlyList<State> GetStateTargets(IDbConnection connection, Type documentType, State fromState)
    {
        var states = DocumentStatesCache(documentType, fromState);
        if (states != null)
        {
            return states.ToList();
        }

        states = GetStateTargetsQuery(connection, documentType, fromState).Get<State>();

        documentStates[documentType.TypeHandle].Add(fromState.Id, states);

        return states.ToList();
    }

    public async Task<IEnumerable<State>> GetStateTargetsAsync(BaseDocument document)
    {
        using var conn = database.OpenConnection();
        return await GetStateTargetsAsync(conn, document);
    }

    public async Task<IEnumerable<State>> GetStateTargetsAsync(IDbConnection connection, BaseDocument document)
    {
        if (document.State != null)
        {
            return await GetStateTargetsAsync(connection, document.GetType(), document.State);
        }

        return Array.Empty<State>();
    }

    public async Task<IEnumerable<State>> GetStateTargetsAsync(Type documentType, State fromState)
    {
        using var conn = database.OpenConnection();
        return await GetStateTargetsAsync(conn, documentType, fromState);
    }

    public async Task<IEnumerable<State>> GetStateTargetsAsync(IDbConnection connection, Type documentType, State fromState)
    {
        var states = DocumentStatesCache(documentType, fromState);
        if (states != null)
        {
            return states.ToList();
        }

        states = await GetStateTargetsQuery(connection, documentType, fromState)
            .GetAsync<State>();

        documentStates[documentType.TypeHandle].Add(fromState.Id, states);

        return states;
    }

    public void SetDocumentState(BaseDocument document, short newState)
    {
        using var conn = database.OpenConnection();
        using var transaction = conn.BeginTransaction();

        try
        {
            SetDocumentState(conn, document, newState, transaction);
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new RepositoryException(ExceptionHelper.Message(e), e);
        }
    }

    public void SetDocumentState(IDbConnection connection, BaseDocument document, short newState, IDbTransaction? transaction)
    {
        connection.Execute($"update {document.GetType().Name.Underscore()} set state_id = :newState where id = :Id", new { newState, document.Id }, transaction);
    }

    public async Task SetDocumentStateAsync(BaseDocument document, short newState)
    {
        using var conn = database.OpenConnection();
        using var transaction = conn.BeginTransaction();

        try
        {
            await SetDocumentStateAsync(conn, document, newState, transaction);
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new RepositoryException(ExceptionHelper.Message(e), e);
        }
    }

    public async Task SetDocumentStateAsync(IDbConnection connection, BaseDocument document, short newState, IDbTransaction? transaction)
    {
        await connection.ExecuteAsync($"update {document.GetType().Name.Underscore()} set state_id = :newState where id = :Id", new { newState, document.Id }, transaction);
    }

    private static Query GetStateTargetsQuery(IDbConnection connection, Type documentType, State fromState)
    {
        QueryParameters parameters = new()
        {
            Alias = "cs",
            IncludeDocumentsInfo = false,
            Quantity = QuantityInformation.None
        };

        return connection.GetQuery<ChangingState>(parameters)
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
            );
    }

    private static IEnumerable<State>? DocumentStatesCache(Type documentType, State fromState)
    {
        if (documentStates.TryGetValue(documentType.TypeHandle, out var typeStates))
        {
            if (typeStates.TryGetValue(fromState.Id, out var states))
            {
                return states.ToList();
            }
        }
        else
        {
            documentStates[documentType.TypeHandle] = new Dictionary<short, IEnumerable<State>>();
        }

        return null;
    }
}
