//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Exceptions;
using DocumentFlow.Data.Models;

using SqlKata;
using SqlKata.Execution;

using System.Data;
using System.Reflection;

namespace DocumentFlow.Common.Extensions;

public static class QueryExtension
{
    public static IEnumerable<TSource> Get<TSource, T>(this Query query, string? foreignName = null, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var foreign = GetForeignProperty<TSource, T>(foreignName);
        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);
        var list = factory.Connection.Query<TSource, T, TSource>(
            compiled.Sql,
            (source, t1) =>
            {
                foreign.SetValue(source, t1);
                return source;
            },
            parameters,
            transaction, 
            commandTimeout: timeout);

        return list;
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2>(this Query query, string? foreignName1 = null, string? foreignName2 = null, IDbTransaction ? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var foreign1 = GetForeignProperty<TSource, T1>(foreignName1);
        var foreign2 = GetForeignProperty<TSource, T2>(foreignName2);

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);
        var list = factory.Connection.Query<TSource, T1, T2, TSource>(
            compiled.Sql,
            (source, t1, t2) =>
            {
                foreign1.SetValue(source, t1);
                foreign2.SetValue(source, t2);
                return source;
            },
            parameters,
            transaction,
            commandTimeout: timeout);

        return list;
    }

    private static PropertyInfo GetForeignProperty<TSource, T>(string? foreignName = null)
    {
        var foreigns = EntityProperties.ForeignPropertiesCache(typeof(TSource)).Where(p => p.PropertyType == typeof(T));
        if (!foreigns.Any())
        {
            throw new ForeignKeyMissing();
        }

        PropertyInfo? foreign = null;
        if (foreignName != null)
        {
            foreign = foreigns.FirstOrDefault(p => p.PropertyType.GetCustomAttributes<ForeignKeyAttribute>().FirstOrDefault(a => a.Name == foreignName) != null);
        }
        else
        {
            foreign = foreigns.FirstOrDefault();
        }

        if (foreign == null)
        {
            throw new ForeignKeyNotFound();
        }

        return foreign;
    }
}
