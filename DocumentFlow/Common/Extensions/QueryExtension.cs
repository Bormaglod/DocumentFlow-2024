//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Humanizer;

using SqlKata;
using SqlKata.Execution;

using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DocumentFlow.Common.Extensions;

public static partial class QueryExtension
{
    public static IEnumerable<TSource> Get<TSource, T>(this Query query, Func<TSource, T, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
        where T : IIdentifier
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);

        List<TSource> listSource = new();
        List<T> listT = new();
        using (var reader = factory.Connection.ExecuteReader(compiled.Sql, parameters, transaction, timeout))
        {
            var fields = GetFields(reader);

            int startIndex = 0;
            var parserSource = GetRowParser<TSource>(reader, fields, ref startIndex);
            var parserT = GetRowParser<T>(reader, fields, ref startIndex);

            while (reader.Read())
            {
                var source = parserSource(reader);
                var t = ParserRow(reader, parserT, listT);

                map(source, t);
                listSource.Add(source);
            }
        }

        return listSource;
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2>(this Query query, Func<TSource, T1, T2, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
        where T1 : IIdentifier
        where T2 : IIdentifier
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);

        List<TSource> listSource = new();
        List<T1> listT1 = new();
        List<T2> listT2 = new();
        using (var reader = factory.Connection.ExecuteReader(compiled.Sql, parameters, transaction, timeout))
        {
            var fields = GetFields(reader);

            int startIndex = 0;
            var parserSource = GetRowParser<TSource>(reader, fields, ref startIndex);
            var parserT1 = GetRowParser<T1>(reader, fields, ref startIndex);
            var parserT2 = GetRowParser<T2>(reader, fields, ref startIndex);

            while (reader.Read())
            {
                var source = parserSource(reader);
                var t1 = ParserRow(reader, parserT1, listT1);
                var t2 = ParserRow(reader, parserT2, listT2);

                map(source, t1, t2);
                listSource.Add(source);
            }
        }

        return listSource;
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2, T3>(this Query query, Func<TSource, T1, T2, T3, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
        where T1 : IIdentifier
        where T2 : IIdentifier
        where T3 : IIdentifier
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);

        List<TSource> listSource = new();
        List<T1> listT1 = new();
        List<T2> listT2 = new();
        List<T3> listT3 = new();
        using (var reader = factory.Connection.ExecuteReader(compiled.Sql, parameters, transaction, timeout))
        {
            var fields = GetFields(reader);

            int startIndex = 0;
            var parserSource = GetRowParser<TSource>(reader, fields, ref startIndex);
            var parserT1 = GetRowParser<T1>(reader, fields, ref startIndex);
            var parserT2 = GetRowParser<T2>(reader, fields, ref startIndex);
            var parserT3 = GetRowParser<T3>(reader, fields, ref startIndex);

            while (reader.Read())
            {
                var source = parserSource(reader);
                var t1 = ParserRow(reader, parserT1, listT1);
                var t2 = ParserRow(reader, parserT2, listT2);
                var t3 = ParserRow(reader, parserT3, listT3);

                map(source, t1, t2, t3);
                listSource.Add(source);
            }
        }

        return listSource;
    }

    public static IEnumerable<TSource> Get<TSource, T1, T2, T3, T4>(this Query query, Func<TSource, T1, T2, T3, T4, TSource> map, IDbTransaction? transaction = null, int? timeout = null)
        where T1 : IIdentifier
        where T2 : IIdentifier
        where T3 : IIdentifier
        where T4 : IIdentifier
    {
        var factory = ((XQuery)query).QueryFactory;

        var compiled = factory.Compiler.Compile(query);
        var parameters = new DynamicParameters(compiled.NamedBindings);

        List<TSource> listSource = new();
        List<T1> listT1 = new();
        List<T2> listT2 = new();
        List<T3> listT3 = new();
        List<T4> listT4 = new();
        using (var reader = factory.Connection.ExecuteReader(compiled.Sql, parameters, transaction, timeout))
        {
            var fields = GetFields(reader);

            int startIndex = 0;
            var parserSource = GetRowParser<TSource>(reader, fields, ref startIndex);
            var parserT1 = GetRowParser<T1>(reader, fields, ref startIndex);
            var parserT2 = GetRowParser<T2>(reader, fields, ref startIndex);
            var parserT3 = GetRowParser<T3>(reader, fields, ref startIndex);
            var parserT4 = GetRowParser<T4>(reader, fields, ref startIndex);

            while (reader.Read())
            {
                var source = parserSource(reader);
                var t1 = ParserRow(reader, parserT1, listT1);
                var t2 = ParserRow(reader, parserT2, listT2);
                var t3 = ParserRow(reader, parserT3, listT3);
                var t4 = ParserRow(reader, parserT4, listT4);

                map(source, t1, t2, t3, t4);
                listSource.Add(source);
            }
        }

        return listSource;
    }

    public static Query MappingQuery<T>(this Query query, Expression<Func<T, object?>> memberExpression, QuantityInformation information = QuantityInformation.Full, string? alias = null)
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

            return query
                .When(!string.IsNullOrEmpty(select), q => q
                    .Select($"{alias}.{select}"))
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

    private static Func<IDataReader, T> GetRowParser<T>(IDataReader reader, string[] fields, ref int startIndex)
    {
        int n1 = Find(fields, startIndex);
        int n2 = Find(fields, n1 + 1);
        if (n2 == -1)
        {
            n2 = fields.Length;
        }

        int len = n2 - n1;

        startIndex = n2;

        return reader.GetRowParser<T>(startIndex: n1, length: len, returnNullIfFirstMissing: true);
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

    private static T ParserRow<T>(IDataReader reader, Func<IDataReader, T> parser, List<T> cache)
        where T : IIdentifier
    {
        var t = parser(reader);
        if (t is Identifier identifier)
        {
            var inT = cache.FirstOrDefault(x => x.Identical(identifier));
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
