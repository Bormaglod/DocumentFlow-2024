//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Exceptions;

using Humanizer;

using SqlKata;
using SqlKata.Execution;

using Syncfusion.Windows.Controls.Input.Resources;

using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace DocumentFlow.Common.Extensions;

public static class QueryExtension
{
    public static IEnumerable<TSource> Get<TSource, T>(this Query query, Func<TSource, T, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);
        var list = factory.Connection.Query(
            compiled.Sql,
            map,
            parameters,
            transaction,
            commandTimeout: timeout);

        return list;
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2>(this Query query, Func<TSource, T1, T2, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);
        var list = factory.Connection.Query(
            compiled.Sql,
            map,
            parameters,
            transaction,
            commandTimeout: timeout);

        return list;
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2, T3>(this Query query, Func<TSource, T1, T2, T3, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);
        var list = factory.Connection.Query(
            compiled.Sql,
            map,
            parameters,
            transaction,
            commandTimeout: timeout);

        return list;
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2, T3, T4>(this Query query, Func<TSource, T1, T2, T3, T4, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);
        var list = factory.Connection.Query(
            compiled.Sql,
            map,
            parameters,
            transaction,
            commandTimeout: timeout);

        return list;
    }

    public static Query MappingQuery<T>(this Query query, Expression<Func<T, object?>> memberExpression, QuantityInformation information = QuantityInformation.Full, string? alias = null)
    {
        alias ??= query.GenerateJoinAlias();

        if (memberExpression.ToMember() is PropertyInfo prop)
        {
            var table = prop.PropertyType.Name.Underscore();

            var refName = $"{table}_id";
            var refTable = query.GetOneComponent<AbstractFrom>("from").Alias ?? "t0";

            var attr = prop.GetCustomAttribute<ForeignKeyAttribute>();
            if (attr != null)
            {
                if (!string.IsNullOrEmpty(attr.FieldKey))
                {
                    refName = attr.FieldKey;
                }

                if (!string.IsNullOrEmpty(attr.Table))
                {
                    refTable = query.GetJoinRefAlias(attr.Table);
                }
            }

            var select = information switch
            {
                QuantityInformation.Full => "*",
                QuantityInformation.Directory => "{id, code, item_name}",
                QuantityInformation.DirectoryExt => "{id, code, item_name, parent_id}",
                _ => throw new NotImplementedException()
            };

            return query
                .Select($"{alias}.{select}")
                .LeftJoin($"{table} as {alias}", $"{alias}.id", $"{refTable}.{refName}");
        }

        throw new Exception("memberExpression должен быть свойством.");
    }

    private static string GenerateJoinAlias(this Query query)
    {
        var aliases = new List<string>();
        var joins = query.GetComponents<BaseJoin>("join");
        foreach (var join in joins)
        {
            var from = join.Join.GetOneComponent<FromClause>("from");
            aliases.Add(from.Alias);
        }

        string alias;
        int n = 1;

        while (aliases.Contains(alias = $"t{n++}")) ;

        return alias;
    }

    private static string GetJoinRefAlias(this Query query, string refTable)
    {
        var joins = query.GetComponents<BaseJoin>("join");
        foreach (var join in joins)
        {
            var from = join.Join.GetOneComponent<FromClause>("from");
            var t = from.Table.Split("as", StringSplitOptions.TrimEntries);
            if (t.Length > 0 && t[0] == refTable)
            {
                return from.Alias;
            }
        }

        throw new Exception($"Join для таблицы {refTable} не найден.");
    }
}
