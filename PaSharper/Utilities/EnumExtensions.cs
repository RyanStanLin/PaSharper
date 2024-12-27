using System.ComponentModel;

namespace PaSharper.Tools;

public static class EnumExtensions
{
    public static string ToCustomString<TEnum>(this TEnum value) where TEnum : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute != null ? attribute.Description : value.ToString();
    }
}