using System.Linq.Expressions;
using System.Reflection;

namespace PaSharper.Extensions;

public static class ObjectExtension
{
    public static bool EqualsExcept<T>(this T obj1, T obj2, params Expression<Func<T, object>>[] excludedProperties)
    {
        if (obj1 == null || obj2 == null)
            throw new ArgumentNullException("Objects to compare cannot be null.");

        var type = typeof(T);
        var excludedPropertyPaths = excludedProperties
            .Select(CreatePropertyPathGetter)
            .ToList();

        return CompareObjects(obj1, obj2, excludedPropertyPaths);
    }

    private static bool CompareObjects(object obj1, object obj2, List<PropertyPathGetter> excludedPropertyPaths)
    {
        if (obj1 == null || obj2 == null) return obj1 == obj2;

        var type = obj1.GetType();

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (excludedPropertyPaths.Any(getter => getter.PropertyPath.Contains(property)))
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
                if (!CompareObjects(value1, value2, excludedPropertyPaths))
                    return false;
            }
        }

        return true;
    }

    private static PropertyPathGetter CreatePropertyPathGetter<T>(Expression<Func<T, object>> property)
    {
        if (property.Body is MemberExpression member ||
            (property.Body is UnaryExpression unary && (member = unary.Operand as MemberExpression) != null))
        {
            var propertyPath = new List<PropertyInfo>();
            while (member != null)
            {
                propertyPath.Insert(0, (PropertyInfo)member.Member);
                member = member.Expression as MemberExpression;
            }

            return new PropertyPathGetter(propertyPath);
        }

        throw new InvalidOperationException("The attribute mapping must be a valid attribute expression.");
    }

    private class PropertyPathGetter
    {
        public PropertyPathGetter(List<PropertyInfo> propertyPath)
        {
            PropertyPath = propertyPath;
        }

        public List<PropertyInfo> PropertyPath { get; }
    }
}