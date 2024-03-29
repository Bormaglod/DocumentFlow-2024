﻿//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;
using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.Repository;

public abstract class ReadOnlyRepository<T> : IReadOnlyRepository<T>
    where T : Identifier
{
    public ReadOnlyRepository(IDatabase database)
    {
        CurrentDatabase = database;
    }

    protected IDatabase CurrentDatabase { get; private set; }

    public IReadOnlyList<T> GetSlim()
    {
        using var conn = GetConnection();
        return GetSlim(conn);
    }

    public IReadOnlyList<T> GetSlim(IDbConnection connection) => GetSlimQuery(connection).Get<T>().ToList();

    protected virtual Query GetSlimQuery(IDbConnection connection)
    {
        return GetQuery(connection, GetDefaultSlimQueryParameters());
    }

    protected virtual QueryParameters GetDefaultSlimQueryParameters() => new() { IncludeDocumentsInfo = false };

    protected Query GetQuery(IDbConnection connection, QueryParameters? parameters = null) => connection.GetQuery<T>(parameters);

    protected IDbConnection GetConnection() => CurrentDatabase.OpenConnection();
}
