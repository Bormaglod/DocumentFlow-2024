//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Models.Entities;

using Humanizer;

using SqlKata;
using SqlKata.Execution;

using System.Collections;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DocumentFlow.Common.Extensions;

public static partial class QueryExtension
{
    public static IEnumerable<TSource> Get<TSource, T>(this Query query, Func<TSource, T, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);

        var types = new Type[] { typeof(TSource), typeof(T) };

        var list = GetListCollection(types);
        using var reader = factory.Connection.ExecuteReader(compiled.Sql, parameters, transaction, timeout);

        var parsers = GetListParsers(reader, types, GetFields(reader));

        while (reader.Read())
        {
            var source = parsers[0](reader);
            var values = GetListValues(reader, parsers, list);

            var result = map((TSource)source, (T)values[0]);
            ((IList)list[0]).Add(result);
        }

        return (IEnumerable<TSource>)list[0];
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2>(this Query query, Func<TSource, T1, T2, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);

        var types = new Type[] { typeof(TSource), typeof(T1), typeof(T2) };

        var list = GetListCollection(types);
        using var reader = factory.Connection.ExecuteReader(compiled.Sql, parameters, transaction, timeout);

        var parsers = GetListParsers(reader, types, GetFields(reader));

        while (reader.Read())
        {
            var source = parsers[0](reader);
            var values = GetListValues(reader, parsers, list);

            var result = map((TSource)source, (T1)values[0], (T2)values[1]);
            ((IList)list[0]).Add(result);
        }

        return (IEnumerable<TSource>)list[0];
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2, T3>(this Query query, Func<TSource, T1, T2, T3, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);

        var types = new Type[] { typeof(TSource), typeof(T1), typeof(T2), typeof(T3) };

        var list = GetListCollection(types);
        using var reader = factory.Connection.ExecuteReader(compiled.Sql, parameters, transaction, timeout);

        var parsers = GetListParsers(reader, types, GetFields(reader));

        while (reader.Read())
        {
            var source = parsers[0](reader);
            var values = GetListValues(reader, parsers, list);

            var result = map((TSource)source, (T1)values[0], (T2)values[1], (T3)values[2]);
            ((IList)list[0]).Add(result);
        }

        return (IEnumerable<TSource>)list[0];
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2, T3, T4>(this Query query, Func<TSource, T1, T2, T3, T4, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);

        var types = new Type[] { typeof(TSource), typeof(T1), typeof(T2), typeof(T3), typeof(T4) };

        var list = GetListCollection(types);
        using var reader = factory.Connection.ExecuteReader(compiled.Sql, parameters, transaction, timeout);

        var parsers = GetListParsers(reader, types, GetFields(reader));

        while (reader.Read())
        {
            var source = parsers[0](reader);
            var values = GetListValues(reader, parsers, list);

            var result = map((TSource)source, (T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3]);
            ((IList)list[0]).Add(result);
        }

        return (IEnumerable<TSource>)list[0];
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2, T3, T4, T5>(this Query query, Func<TSource, T1, T2, T3, T4, T5, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);

        var types = new Type[] { typeof(TSource), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };

        var list = GetListCollection(types);
        using var reader = factory.Connection.ExecuteReader(compiled.Sql, parameters, transaction, timeout);

        var parsers = GetListParsers(reader, types, GetFields(reader));

        while (reader.Read())
        {
            var source = parsers[0](reader);
            var values = GetListValues(reader, parsers, list);

            var result = map((TSource)source, (T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3], (T5)values[4]);
            ((IList)list[0]).Add(result);
        }

        return (IEnumerable<TSource>)list[0];
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2, T3, T4, T5, T6>(this Query query, Func<TSource, T1, T2, T3, T4, T5, T6, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);

        var types = new Type[] { typeof(TSource), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };

        var list = GetListCollection(types);
        using var reader = factory.Connection.ExecuteReader(compiled.Sql, parameters, transaction, timeout);

        var parsers = GetListParsers(reader, types, GetFields(reader));

        while (reader.Read())
        {
            var source = parsers[0](reader);
            var values = GetListValues(reader, parsers, list);

            var result = map((TSource)source, (T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3], (T5)values[4], (T6)values[5]);
            ((IList)list[0]).Add(result);
        }

        return (IEnumerable<TSource>)list[0];
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2, T3, T4, T5, T6, T7>(this Query query, Func<TSource, T1, T2, T3, T4, T5, T6, T7, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);

        var types = new Type[] { typeof(TSource), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };

        var list = GetListCollection(types);
        using var reader = factory.Connection.ExecuteReader(compiled.Sql, parameters, transaction, timeout);

        var parsers = GetListParsers(reader, types, GetFields(reader));

        while (reader.Read())
        {
            var source = parsers[0](reader);
            var values = GetListValues(reader, parsers, list);

            var result = map((TSource)source, (T1)values[0], (T2)values[1], (T3)values[2], (T4)values[3], (T5)values[4], (T6)values[5], (T7)values[6]);
            ((IList)list[0]).Add(result);
        }

        return (IEnumerable<TSource>)list[0];
    }

    public static Query MappingQuery<T>(this Query query, Expression<Func<T, object?>> memberExpression, QuantityInformation information = QuantityInformation.Full, string? alias = null, JoinType joinType = JoinType.Left)
    {
        alias ??= query.GenerateJoinAlias();

        if (memberExpression.ToMember() is PropertyInfo prop)
        {
            var table = prop.PropertyType.Name.Underscore();

            var refName = $"{table}_id";
            var refTable = query.GetAliasTable(typeof(T).Name.Underscore());

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
                QuantityInformation.Id => "id",
                QuantityInformation.Directory => "{id, code, item_name}",
                QuantityInformation.DirectoryExt => "{id, code, item_name, parent_id}",
                QuantityInformation.None => string.Empty,
                _ => throw new NotImplementedException()
            };

            var curTable = $"{table} as {alias}";
            var columnFirst = $"{alias}.id";
            var columnSecond = $"{refTable}.{refName}";

            return query
                .When(!string.IsNullOrEmpty(select), q => q
                    .Select($"{alias}.{select}"))
                .When(joinType == JoinType.Left, q => 
                    q.LeftJoin(curTable, columnFirst, columnSecond))
                .When(joinType == JoinType.Right, q => 
                    q.RightJoin(curTable, columnFirst, columnSecond))
                .When(joinType == JoinType.Inner, q => 
                    q.Join(curTable, columnFirst, columnSecond));
        }

        throw new Exception("memberExpression должен быть свойством.");
    }

    private static string GenerateJoinAlias(this Query query)
    {
        var aliases = new List<string>();
        var joins = query.GetComponents<BaseJoin>("join");
        foreach (var join in joins)
        {
            var from = join.Join.GetOneComponent<AbstractFrom>("from");
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
            var joinTable = TableNameRegex().Match(from.Table).Value;
            if (string.Compare(joinTable, refTable, true) == 0)
            {
                return from.Alias;
            }
        }

        throw new Exception($"Join для таблицы {refTable} не найден.");
    }

    private static string GetAliasTable(this Query query, string table)
    {
        var fromComponent = query.GetOneComponent<AbstractFrom>("from");
        if (fromComponent is FromClause fromClause)
        {
            var fromTable = TableNameRegex().Match(fromClause.Table).Value;
            if (string.Compare(fromTable, table, true) == 0)
            {
                return fromClause.Alias;
            }
        }

        try
        {
            return query.GetJoinRefAlias(table);
        }
        catch
        {
            return fromComponent.Alias ?? "t0";
        }
    }

    private static string[] GetFields(IDataReader reader)
    {
        string[] fields = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
        {
            fields[i] = reader.GetName(i);
        }

        return fields;
    }

    private static IList<object> GetListCollection(Type[] types)
    {
        var list = new List<object>();
        for (int i = 0; i < types.Length; i++)
        {
            var listType = typeof(List<>);
            var concreteType = listType.MakeGenericType(types[i]);
            var newList = Activator.CreateInstance(concreteType);

            if (newList == null)
            {
                throw new NullReferenceException(nameof(newList));
            }

            list.Add(newList);
        }

        return list;
    }

    private static IList<Func<IDataReader, object>> GetListParsers(IDataReader reader, Type[] types, string[] fields)
    {
        int startIndex = 0;
        var list = new List<Func<IDataReader, object>>();
        for (int i = 0; i < types.Length; i++)
        {
            var parser = GetRowParser(reader, types[i], fields, ref startIndex);
            list.Add(parser);
        }

        return list;
    }

    private static IList<object> GetListValues(IDataReader reader, IList<Func<IDataReader, object>> parsers, IList<object> cache)
    {
        var list = new List<object>();
        for (int i = 1; i < parsers.Count; i++)
        {
            list.Add(ParserRow(reader, parsers[i], (IList)cache[i]));
        }

        return list;
    }

    private static int Find(string[] fields, int startIndex)
    {
        for (int i = startIndex; i < fields.Length; i++)
        {
            if (fields[i] == "id")
            {
                return i;
            }
        }

        return -1;
    }

    private static Func<IDataReader, object> GetRowParser(IDataReader reader, Type type, string[] fields, ref int startIndex)
    {
        int n1 = Find(fields, startIndex);
        int n2 = Find(fields, n1 + 1);
        if (n2 == -1)
        {
            n2 = fields.Length;
        }

        int len = n2 - n1;

        startIndex = n2;

        return reader.GetRowParser(type, startIndex: n1, length: len, returnNullIfFirstMissing: true);
    }

    private static object ParserRow(IDataReader reader, Func<IDataReader, object> parser, IList cache)
    {
        var t = parser(reader);
        if (t is Identifier identifier)
        {
            var inT = cache.OfType<Identifier>().FirstOrDefault(x => x.Identical(identifier));
            if (inT != null)
            {
                t = inT;
            }
            else
            {
                cache.Add(t);
            }
        }

        return t;
    }

    [GeneratedRegex("^\\w+")]
    private static partial Regex TableNameRegex();
}
