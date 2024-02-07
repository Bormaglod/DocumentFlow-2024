//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common.Data;

using SqlKata;
using SqlKata.Execution;

using System.Data;
using System.Reflection;

namespace DocumentFlow.Common.Extensions;

public static class QueryExtension
{
    public static IEnumerable<TSource> Get<TSource, T1>(this Query query, string? foreignName = null, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var foreigns = EntityProperties.ForeignPropertiesCache(typeof(TSource)).Where(p => p.PropertyType == typeof(T1));
        if (!foreigns.Any()) 
        {
            return query.Get<TSource>();
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
            throw new NullReferenceException();
        }

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);
        var list = factory.Connection.Query<TSource, T1, TSource>(
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
}
