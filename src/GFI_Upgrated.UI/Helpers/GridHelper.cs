using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GFI_Upgrated.UI.Helpers
{
    public static class GridHelper
    {
        public static IEnumerable<T> FilterAllColumns<T>(this IEnumerable<T> source, string? searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return source;

            searchString = searchString.Trim();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && (p.PropertyType.IsPrimitive || 
                                         p.PropertyType == typeof(string) || 
                                         Nullable.GetUnderlyingType(p.PropertyType) != null || 
                                         p.PropertyType == typeof(decimal) || 
                                         p.PropertyType == typeof(DateTime) ||
                                         p.PropertyType == typeof(double) ||
                                         p.PropertyType == typeof(float) ||
                                         p.PropertyType == typeof(long) ||
                                         p.PropertyType == typeof(int) ||
                                         p.PropertyType == typeof(short)))
                .ToList();

            return source.Where(item =>
            {
                if (item == null) return false;
                foreach (var prop in properties)
                {
                    try
                    {
                        var val = prop.GetValue(item);
                        if (val != null && val.ToString()!.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                    catch { }
                }
                return false;
            });
        }

        public static IEnumerable<T> SortAllColumns<T>(this IEnumerable<T> source, string? sortLabel, string? sortOrd)
        {
            if (string.IsNullOrWhiteSpace(sortLabel))
                return source;

            var prop = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.Name.Equals(sortLabel, StringComparison.OrdinalIgnoreCase));

            if (prop == null)
                return source;

            bool desc = string.Equals(sortOrd, "DESC", StringComparison.OrdinalIgnoreCase);

            return desc 
                ? source.OrderByDescending(x => prop.GetValue(x) ?? GetDefaultValue(prop.PropertyType))
                : source.OrderBy(x => prop.GetValue(x) ?? GetDefaultValue(prop.PropertyType));
        }

        private static object? GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);
            return null;
        }
    }
}
