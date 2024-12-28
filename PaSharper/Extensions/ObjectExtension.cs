using System.Reflection;
using PaSharper.Data;

namespace PaSharper.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    /// Compares two objects of the same type for equality, ignoring specified properties.
    /// </summary>
    /// <typeparam name="T">The type of objects being compared.</typeparam>
    /// <param name="obj1">The first object to compare.</param>
    /// <param name="obj2">The second object to compare.</param>
    /// <param name="excludedProperties">The names of properties to exclude from comparison.</param>
    /// <returns>True if all properties (except excluded ones) are equal; otherwise, false.</returns>
    public static bool EqualsExcept<T>(this T obj1, T obj2, params string[] excludedProperties)
    {
        if (obj1 == null || obj2 == null)
            throw new ArgumentNullException("Objects to compare cannot be null.");

        var type = obj1.GetType() == obj2.GetType() ? obj1.GetType() : typeof(T);
        //var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (excludedProperties.Contains(property.Name))
                continue;

            var value1 = property.GetValue(obj1);
            var value2 = property.GetValue(obj2);

            if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
            {
                if (!Equals(value1, value2))
                    return false;
            }
            else
            {
                if (value1 == null || value2 == null)
                {
                    if (value1 != value2)
                        return false;
                }
                else if (!value1.EqualsExcept(value2, excludedProperties))
                {
                    return false;
                }
            }
        }

        return true;
    }
}