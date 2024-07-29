using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class TypeUtilities
{
    public static List<T> GetAllPublicConstantValues<T>(this Type type)
    {
        return type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
            .Select(x => (T)x.GetRawConstantValue())
            .ToList();
    }

    /// <summary>
    /// Checks if a given type inherits or implements a specified base type.
    /// </summary>
    /// <param name="type">The type which needs to be checked.</param>
    /// <param name="baseType">The base type/interface which is expected to be inherited or implemented by the 'type'</param>
    /// <returns>Return true if 'type' inherits or implements 'baseType'. False otherwise</returns>        
    public static bool InheritsOrImplements(this Type type, Type baseType)
    {
        type = ResolveGenericType(type);
        baseType = ResolveGenericType(baseType);
        while (type != typeof(object))
        {
            if (baseType == type || HasAnyInterfaces(type, baseType)) return true;
            type = ResolveGenericType(type.BaseType);
            if (type == null) return false;
        }

        return false;
    }

    static Type ResolveGenericType(Type type)
    {
        if (type is not { IsGenericType: true }) return type;
        var genericType = type.GetGenericTypeDefinition();
        return genericType != type ? genericType : type;
    }

    static bool HasAnyInterfaces(Type type, Type intefaceType)
    {
        return type.GetInterfaces().Any(i => ResolveGenericType(i) == intefaceType);
    }

    public static T CopyClass<T>(T obj)
    {
        return (T)JsonHelper.FromJson(JsonHelper.ToJson(obj), obj.GetType());
        // T objCpy = (T)Activator.CreateInstance(obj.GetType());
        // obj.GetType().GetProperties().ToList()
        //     .ForEach(p =>
        //     {
        //         objCpy.GetType().GetProperty(p.Name)?.SetValue(objCpy, p.GetValue(obj));
        //     });
        // return objCpy;
    }
}