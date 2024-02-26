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

using Microsoft.VisualBasic;

using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DocumentFlow.Common.Extensions;

public static class ConnectionExtension
{
    public static Query GetQuery<T>(this IDbConnection connection, QueryParemeters? parameters = null)
    {
        var factory = new QueryFactory(connection, new PostgresCompiler());

        parameters ??= QueryParemeters.Default;

        var table = parameters.Table ?? typeof(T).Name.Underscore();

        Query query;
        if (parameters.FromOnly)
        {
            query = factory.Query().FromRaw($"only {table} as {parameters.Alias}");
        }
        else
        {
            query = factory.Query($"{table} as {parameters.Alias}");
        }

        var select = parameters.Quantity switch
        {
            QuantityInformation.Full => "*",
            QuantityInformation.Id => "id",
            QuantityInformation.Directory => "{id, code, item_name}",
            QuantityInformation.DirectoryExt => "{id, code, item_name, parent_id}",
            QuantityInformation.None => string.Empty,
            _ => throw new NotImplementedException()
        };

        query.When(!string.IsNullOrEmpty(select), q => q.Select($"{parameters.Alias}.{select}"));

        return query;
    }

    public static void UpdateDependents(this IDbConnection connection, object collection, IDbTransaction? transaction = null)
    {
        if (collection is IDependentCollection dependent)
        {
            connection.Insert(dependent.NewItems, transaction);
            connection.Update(dependent.UpdateItems, transaction);
            connection.ExecuteCommand(SQLCommand.Wipe, dependent.RemoveItems, transaction);

            dependent.CompleteChanged();
        }
    }

    public static int Insert(this IDbConnection connection, object entity, IDbTransaction? transaction = null, int? commandTimeout = null)
    {
        if (entity is IEnumerable<object> entities)
        {
            int rowsAffected = 0;

            var entityGroups = entities.GroupBy(x => x.GetType());
            foreach (var grp in entityGroups)
            {
                if (grp.Any()) 
                {
                    if (grp.First() is IDiscriminator entityGroup)
                    {
                        foreach (var d in grp.Cast<IDiscriminator>().GroupBy(x => x.Discriminator))
                        {
                            rowsAffected += connection.InsertInternal(d.ToList(), transaction, commandTimeout);
                        }
                    }
                    else
                    {
                        rowsAffected += connection.InsertInternal(grp.ToList(), transaction, commandTimeout);
                    }
                }
            }

            return rowsAffected;
        }

        return connection.InsertInternal(entity, transaction, commandTimeout);
    }

    public static bool Copy(this IDbConnection connection, object entity, [MaybeNullWhen(false)] out object copy, IDbTransaction? transaction = null, int? commandTimeout = null)
    {
        if (entity is IEnumerable<object>)
        {
            throw new Exception("The entity parameter cannot be an enumeration.");
        }

        return connection.CopyInternal(entity, out copy, transaction, commandTimeout);
    }

    public static bool Update(this IDbConnection connection, object entity, IDbTransaction? transaction = null, int? commandTimeout = null)
    {
        if (entity is IEnumerable<object> entities)
        {
            int rowsAffected = 0;

            var entityGroups = entities.GroupBy(x => x.GetType());
            foreach (var grp in entityGroups)
            {
                if (grp.Any())
                {
                    if (grp.First() is IDiscriminator entityGroup)
                    {
                        foreach (var d in grp.Cast<IDiscriminator>().GroupBy(x => x.Discriminator))
                        {
                            rowsAffected += connection.UpdateInternal(d.ToList(), transaction, commandTimeout);
                        }
                    }
                    else
                    {
                        rowsAffected += connection.UpdateInternal(grp.ToList(), transaction, commandTimeout);
                    }
                }
            }

            return rowsAffected > 0;
        }

        return connection.UpdateInternal(entity, transaction, commandTimeout) > 0;
    }

    public static bool ExecuteCommand(this IDbConnection connection, SQLCommand method, object entity, IDbTransaction? transaction = null, int? commandTimeout = null)
    {
        if (entity is IEnumerable<object> entities)
        {
            int rowsAffected = 0;

            var entityGroups = entities.GroupBy(x => x.GetType());
            foreach (var grp in entityGroups)
            {
                if (grp.Any())
                {
                    if (grp.First() is IDiscriminator entityGroup)
                    {
                        foreach (var d in grp.Cast<IDiscriminator>().GroupBy(x => x.Discriminator))
                        {
                            rowsAffected += connection.ExecuteCommandInternal(method, d.ToList(), transaction, commandTimeout);
                        }
                    }
                    else
                    {
                        rowsAffected += connection.ExecuteCommandInternal(method, grp.ToList(), transaction, commandTimeout);
                    }
                }
            }

            return rowsAffected > 0;
        }

        return connection.ExecuteCommandInternal(method, entity, transaction, commandTimeout) > 0;
    }

    private static int InsertInternal(this IDbConnection connection, object entity, IDbTransaction? transaction = null, int? commandTimeout = null)
    {
        if (IncorrectEntityType(entity, out Type type))
        {
            return 0;
        }

        var name = EntityProperties.GetTableName(type);
        var allProperties = EntityProperties.TypePropertiesCache(type);
        var keyProperties = EntityProperties.KeyPropertiesCache(type);
        var denyWritingProperties = EntityProperties.DenyWritingPropertiesCache(type);
        var enums = EntityProperties.EnumsPropertiesCache(type);
        var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(denyWritingProperties)).ToList();

        var fields = string.Join(", ", allPropertiesExceptKeyAndComputed.Select(x =>
        {
            string name;
            if (x.PropertyType.IsSubclassOf(typeof(DocumentInfo)))
            {
                var attr = x.GetCustomAttribute<ForeignKeyAttribute>(true);
                name = attr == null || string.IsNullOrEmpty(attr.FieldKey) ? x.Name.Underscore() + "_id" : attr.FieldKey;
            }
            else
            {
                name = x.Name.Underscore();
            }

            return name;
        }));

        var values = string.Join(", ", allPropertiesExceptKeyAndComputed.Select(x =>
        {
            string name;
            if (x.PropertyType.IsSubclassOf(typeof(DocumentInfo)))
            {
                var attr = x.GetCustomAttribute<ForeignKeyAttribute>(true);
                name = attr == null || string.IsNullOrEmpty(attr.PropertyKey) ? x.Name + "Id" : attr.PropertyKey;
            }
            else
            {
                name = x.Name;
            }

            return $":{name}{(enums.TryGetValue(x, out string? value) ? "::" + value : "")}";
        }));

        var returning = keyProperties.Count == 0 ? "*" : string.Join(", ", keyProperties.Select(x => x.Name.Underscore()));

        var discriminator = GetDiscriminator(entity);

        var cmd = $"insert into {name}{discriminator} ({fields}) values ({values}) returning {returning}";

        int rowsAffected = 0;
        if (entity is IEnumerable e)
        {
            foreach (var entityItem in e)
            {
                var results = connection.QuerySingle(cmd, GetParameters(entityItem, allProperties), transaction, commandTimeout);
                PopulateIdValue(entityItem, results, keyProperties);
                rowsAffected++;
            }
        }
        else
        {
            var results = connection.QuerySingle(cmd, GetParameters(entity, allProperties), transaction, commandTimeout);
            PopulateIdValue(entity, results, keyProperties);
            rowsAffected = 1;
        }

        return rowsAffected;
    }

    private static bool CopyInternal(this IDbConnection connection, object entity, [MaybeNullWhen(false)] out object copy, IDbTransaction? transaction = null, int? commandTimeout = null)
    {
        if (IncorrectEntityType(entity, out Type type))
        {
            copy = null;
            return false;
        }

        var name = EntityProperties.GetTableName(type);
        var allProperties = EntityProperties.TypePropertiesCache(type);
        var keyProperties = EntityProperties.KeyPropertiesCache(type);
        var denyWritingProperties = EntityProperties.DenyWritingPropertiesCache(type);
        var deniesProperties = EntityProperties.DeniesPropertiesCache(type);
        var enums = EntityProperties.EnumsPropertiesCache(type);
        var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(denyWritingProperties).Union(deniesProperties)).ToList();

        var fields = string.Join(", ", allPropertiesExceptKeyAndComputed.Select(x =>
        {
            string name;
            if (x.PropertyType.IsSubclassOf(typeof(DocumentInfo)))
            {
                var attr = x.GetCustomAttribute<ForeignKeyAttribute>(true);
                name = attr == null || string.IsNullOrEmpty(attr.FieldKey) ? x.Name.Underscore() + "_id" : attr.FieldKey;
            }
            else
            {
                name = x.Name.Underscore();
            }

            return name;
        }));

        var values = string.Join(", ", allPropertiesExceptKeyAndComputed.Select(x =>
        {
            string name;
            if (x.PropertyType.IsSubclassOf(typeof(DocumentInfo)))
            {
                var attr = x.GetCustomAttribute<ForeignKeyAttribute>(true);
                name = attr == null || string.IsNullOrEmpty(attr.PropertyKey) ? x.Name + "Id" : attr.PropertyKey;
            }
            else
            {
                name = x.Name;
            }

            return $":{name}{(enums.TryGetValue(x, out string? value) ? "::" + value : "")}";
        }));

        var returning = keyProperties.Count == 0 ? "*" : string.Join(", ", keyProperties.Select(x => x.Name.Underscore()));

        var discriminator = GetDiscriminator(entity);

        var cmd = $"insert into {name}{discriminator} ({fields}) values ({values}) returning {returning}";

        var results = connection.QuerySingle(cmd, GetParameters(entity, allProperties), transaction, commandTimeout);

        copy = Activator.CreateInstance(type);

        if (copy == null)
        {
            throw new Exception($"Не удалось создать объект типа {type.Name}");
        }

        PopulateIdValue(copy, results, keyProperties);

        return true;
    }

    private static int UpdateInternal(this IDbConnection connection, object entity, IDbTransaction? transaction = null, int? commandTimeout = null)
    {
        if (IncorrectEntityType(entity, out Type type))
        {
            return 0;
        }

        var keyProperties = EntityProperties.KeyPropertiesCache(type);
        if (keyProperties.Count == 0)
        {
            throw new ArgumentException("Entity must have at least one [Key] property");
        }

        var name = EntityProperties.GetTableName(type);

        var allProperties = EntityProperties.TypePropertiesCache(type);
        var denyWritingProperties = EntityProperties.DenyWritingPropertiesCache(type);
        var nonIdProps = allProperties.Except(keyProperties.Union(denyWritingProperties)).ToList();
        var enums = EntityProperties.EnumsPropertiesCache(type);
        var keys = string.Join(" and ", keyProperties.Select(x => $"{x.Name.Underscore()} = :{x.Name}"));

        var sets = string.Join(", ", nonIdProps.Select(x =>
        {
            string name;
            if (x.PropertyType.IsSubclassOf(typeof(DocumentInfo)))
            {
                var attr = x.GetCustomAttribute<ForeignKeyAttribute>(true);
                var fname = attr == null || string.IsNullOrEmpty(attr.FieldKey) ? $"{x.Name.Underscore()}_id" : attr.FieldKey;
                var sname = attr == null || string.IsNullOrEmpty(attr.PropertyKey) ? $"{x.Name}Id" : attr.PropertyKey;

                name = $"{fname} = :{sname}";
            }
            else
            {
                name = $"{x.Name.Underscore()} = :{x.Name}{(enums.TryGetValue(x, out string? value) ? "::" + value : "")}";
            }

            return name;
        }));

        var discriminator = GetDiscriminator(entity);

        var cmd = $"update {name}{discriminator} set {sets} where {keys}";

        int rowsAffected = 0;
        if (entity is IEnumerable e)
        {
            foreach (var entityItem in e)
            {
                rowsAffected += connection.Execute(cmd, GetParameters(entityItem, allProperties), transaction, commandTimeout);
            }
        }
        else
        {
            rowsAffected = connection.Execute(cmd, GetParameters(entity, allProperties), transaction, commandTimeout);
        }

        return rowsAffected;
    }

    public static int ExecuteCommandInternal(this IDbConnection connection, SQLCommand method, object entity, IDbTransaction? transaction = null, int? commandTimeout = null)
    {
        if (IncorrectEntityType(entity, out Type type))
        {
            return 0;
        }

        var keyProperties = EntityProperties.KeyPropertiesCache(type);
        if (keyProperties.Count == 0)
        {
            throw new ArgumentException("Entity must have at least one [Key]");
        }

        var name = EntityProperties.GetTableName(type);
        var keys = string.Join(" and ", keyProperties.Select(x => $"{x.Name.Underscore()} = :{x.Name}"));

        var discriminator = GetDiscriminator(entity);

        string cmd = method switch
        {
            SQLCommand.Delete => $"update {name}{discriminator} set deleted = true where {keys}",
            SQLCommand.Undelete => $"update {name}{discriminator} set deleted = false where {keys}",
            SQLCommand.Wipe => $"delete from {name}{discriminator} where {keys}",
            _ => throw new NotImplementedException()
        };

        int rowsAffected = 0;
        if (entity is IEnumerable e)
        {
            foreach (var entityItem in e)
            {
                rowsAffected += connection.Execute(cmd, entityItem, transaction, commandTimeout);
            }
        }
        else
        {
            rowsAffected = connection.Execute(cmd, entity, transaction, commandTimeout);
        }

        return rowsAffected;
    }

    private static void PopulateIdValue(object entity, dynamic results, IEnumerable<PropertyInfo> keyProperties)
    {
        foreach (var p in keyProperties)
        {
            var value = ((IDictionary<string, object>)results)[p.Name.Underscore()];
            p.SetValue(entity, value, null);
        }
    }

    private static bool IncorrectEntityType(object entity, out Type type)
    {
        type = entity.GetType();

        if (entity is IList list)
        {
            if (list.Count == 0)
            {
                return true;
            }

            type = list[0]!.GetType();
        }

        return false;
    }

    private static string GetDiscriminator(object entity) 
    { 
        if (entity is IEnumerable<object> entities)
        {
            if (entities.Any()) 
            {
                entity = entities.First();
            }
            else
            {
                return string.Empty;
            }
        }

        if (entity is IDiscriminator discriminator)
        {
            if (string.IsNullOrEmpty(discriminator.Discriminator))
            {
                ArgumentNullException.ThrowIfNull(discriminator.Discriminator);
            }
            
            return $"_{discriminator.Discriminator}";
        }
        else
        {
            return string.Empty;
        }
    }

    private static DynamicParameters GetParameters(object entity, IEnumerable<PropertyInfo> props)
    {
        var parameters = new DynamicParameters();
        foreach (var item in props)
        {
            var val = item.GetValue(entity);
            if (val is DocumentInfo document)
            {
                var attr = item.GetCustomAttribute<ForeignKeyAttribute>(true);
                if (attr == null || string.IsNullOrEmpty(attr.PropertyKey))
                {
                    parameters.Add($"{item.Name}Id", document.Id);
                }
                else
                {
                    parameters.Add(attr.PropertyKey, document.Id);
                }
            }
            else
            {
                if (item.PropertyType.IsSubclassOf(typeof(Identifier)))
                {
                    parameters.Add($"{item.Name}Id", val);
                }
                else
                {
                    parameters.Add(item.Name, val);
                }
            }
        }

        return parameters;
    }
}
