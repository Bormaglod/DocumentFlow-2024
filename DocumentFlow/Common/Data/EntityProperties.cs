//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Models.Entities;

using Humanizer;

using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace DocumentFlow.Common.Data;

internal static class EntityProperties
{
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> DenyWritingProperties = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IDictionary<PropertyInfo, string>> EnumsProperties = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> DenyCopyingProperties = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ForeignProperties = new();

    internal static string GetTableName(Type type)
    {
        if (TypeTableName.TryGetValue(type.TypeHandle, out string? name))
        {
            return name;
        }

        var tableAttrName = type.GetCustomAttribute<TableAttribute>(false)?.Name;

        if (tableAttrName != null)
        {
            name = tableAttrName;
        }
        else
        {
            name = type.Name.Underscore();
        }

        TypeTableName[type.TypeHandle] = name;

        return name;
    }

    internal static List<PropertyInfo> TypePropertiesCache(Type type)
    {
        if (TypeProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo>? pis))
        {
            return pis.ToList();
        }

        var properties = type.GetProperties().Where(IsWriteable).ToArray();
        TypeProperties[type.TypeHandle] = properties;
        return properties.ToList();
    }

    internal static List<PropertyInfo> KeyPropertiesCache(Type type)
    {
        if (KeyProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo>? pi))
        {
            return pi.ToList();
        }

        var allProperties = TypePropertiesCache(type);
        var keyProperties = allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

        if (keyProperties.Count == 0)
        {
            var idProp = allProperties.Find(p => string.Equals(p.Name, "id", StringComparison.CurrentCultureIgnoreCase));
            if (idProp != null)
            {
                keyProperties.Add(idProp);
            }
        }

        KeyProperties[type.TypeHandle] = keyProperties;
        return keyProperties;
    }

    internal static List<PropertyInfo> DenyWritingPropertiesCache(Type type)
    {
        if (DenyWritingProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo>? pi))
        {
            return pi.ToList();
        }

        var computedProperties = TypePropertiesCache(type).Where(p => p.GetCustomAttributes(true).Any(a => a is DenyWritingAttribute)).ToList();

        DenyWritingProperties[type.TypeHandle] = computedProperties;
        return computedProperties;
    }

    internal static Dictionary<PropertyInfo, string> EnumsPropertiesCache(Type type)
    {
        if (EnumsProperties.TryGetValue(type.TypeHandle, out IDictionary<PropertyInfo, string>? pi))
        {
            return (Dictionary<PropertyInfo, string>)pi;
        }

        var enumsProperties = new Dictionary<PropertyInfo, string>();
        foreach (var item in TypePropertiesCache(type))
        {
            var attr = item.GetCustomAttribute<EnumTypeAttribute>(true);
            if (attr != null)
            {
                enumsProperties.Add(item, attr.Name);
            }
        }

        EnumsProperties[type.TypeHandle] = enumsProperties;
        return enumsProperties;
    }

    internal static List<PropertyInfo> DeniesPropertiesCache(Type type)
    {
        if (DenyCopyingProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo>? pi))
        {
            return pi.ToList();
        }

        var deniesProperties = TypePropertiesCache(type).Where(p => p.GetCustomAttributes(true).Any(a => a is DenyCopyingAttribute)).ToList();

        DenyCopyingProperties[type.TypeHandle] = deniesProperties;
        return deniesProperties;
    }

    internal static List<PropertyInfo> ForeignPropertiesCache(Type type)
    {
        if (ForeignProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo>? pi))
        {
            return pi.ToList();
        }

        var foreignProperties = TypePropertiesCache(type).Where(p => p.PropertyType.IsAssignableTo(typeof(DocumentInfo))).ToList();
        
        ForeignProperties[type.TypeHandle] = foreignProperties;
        return foreignProperties;
    }

    private static bool IsWriteable(PropertyInfo pi)
    {
        var attributes = pi.GetCustomAttributes(typeof(WriteAttribute), false);
        if (attributes.Length != 1)
        {
            return pi.SetMethod?.IsPublic ?? false;
        }

        var writeAttribute = (WriteAttribute)attributes[0];
        return writeAttribute.Write;
    }
}
