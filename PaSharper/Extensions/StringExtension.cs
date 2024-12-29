using System.Text;

namespace PaSharper.Extensions;

public static class StringExtension
{
    /// <summary>
    /// 将 IEnumerable<string> 转换为单个字符串，并将每一项直接拼接起来。
    /// </summary>
    /// <param name="source">字符串集合</param>
    /// <returns>拼接后的字符串</returns>
    public static string JoinToString(this IEnumerable<string> source)
    {
        if (source == null) 
            throw new ArgumentNullException(nameof(source), "Source collection cannot be null.");

        var stringBuilder = new StringBuilder();
        
        foreach (var str in source)
        {
            stringBuilder.Append(str); // 直接拼接，没有分隔符
        }

        return stringBuilder.ToString();
    }
}