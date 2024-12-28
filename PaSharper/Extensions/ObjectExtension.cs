using System;
using System.Linq;
using System.Reflection;

namespace PaSharper.Tools;

public static class ObjectExtensions
{
    public static bool EqualsExcept<T>(this T obj1, T obj2, params string[] excludedProperties)
    {
        if (obj1 == null || obj2 == null)
            throw new ArgumentNullException("Objects to compare cannot be null.");

        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (excludedProperties.Contains(property.Name))
                continue;

            var value1 = property.GetValue(obj1);
            var value2 = property.GetValue(obj2);

            if (!Equals(value1, value2))
                return false;
        }

        return true;
    }
}