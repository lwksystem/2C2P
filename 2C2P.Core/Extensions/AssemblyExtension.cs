using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _2C2P.Core.Attributes;

namespace _2C2P.Core.Extensions
{
    public static class AssemblyExtension
    {
        public static IEnumerable<TypeAttribute<TAttribute>> GetTypesWithAttribute<TAttribute>(this IEnumerable<Assembly> assemblies)
         where TAttribute : Attribute
        {
            var typesWithAttribute =
                from a in assemblies.AsParallel()
                from t in a.GetTypes().AsParallel()
                let attributes = t.GetCustomAttributes(typeof(TAttribute), true)
                where attributes != null && attributes.Length > 0
                select new TypeAttribute<TAttribute> { Type = t, Attributes = attributes.Cast<TAttribute>() };

            return typesWithAttribute;
        }

        public static IEnumerable<TypeAttribute<TAttribute>> GetTypesWithAttribute<TAttribute, TType>(this IEnumerable<Assembly> assemblies)
            where TAttribute : Attribute
        {
            var typesWithAttribute =
                from a in assemblies.AsParallel()
                from t in a.GetTypes().AsParallel()
                where typeof(TType).IsAssignableFrom(t)
                let attributes = t.GetCustomAttributes(typeof(TAttribute), true)
                where attributes != null && attributes.Length > 0
                select new TypeAttribute<TAttribute> { Type = t, Attributes = attributes.Cast<TAttribute>() };

            return typesWithAttribute;
        }

        public static IEnumerable<TypeAttribute<TAttribute>> GetTypesWithAttribute<TAttribute>(this IEnumerable<Assembly> assemblies, Type type)
            where TAttribute : Attribute
        {
            var typesWithAttribute =
                from a in assemblies.AsParallel()
                from t in a.GetTypes().AsParallel()
                where type.IsAssignableFrom(t)
                let attributes = t.GetCustomAttributes(typeof(TAttribute), true)
                where attributes != null && attributes.Length > 0
                select new TypeAttribute<TAttribute> { Type = t, Attributes = attributes.Cast<TAttribute>() };

            return typesWithAttribute;
        }
    }
}