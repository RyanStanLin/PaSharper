using System.Text.RegularExpressions;
using System.Collections.Generic;
using PaSharper.Utilities.File;

namespace PaSharper.Utilities.WorkFlow.Implements.IGCSEs.Utilities;

public static class TotalMatcher
{
    /// <summary>
    /// 在输入的 TextChunk 列表中匹配 [Total: {数字}] 格式的内容。
    /// </summary>
    /// <param name="textChunks">输入的 TextChunk 列表</param>
    /// <returns>返回所有匹配到的 Total 数字及其对应的 TextChunk 列表</returns>
    public static List<(int totalMark, List<T> textChunks)> FindTotals<T>(IEnumerable<T> textChunks) where T : PdfTextExtractor.TextChunk
    {
        var totals = new List<(int totalMark, List<T> textChunks)>();

        if (textChunks == null)
            throw new ArgumentNullException(nameof(textChunks), "输入的 TextChunk 列表不能为空");

        // 创建一个临时列表来存储当前行的 TextChunk
        List<T> currentLineChunks = new List<T>();
        foreach (var chunk in textChunks)
        {
            // 使用正则表达式检查 TextChunk 的文本是否符合 [Total: xxx] 格式
            var pattern = @"\[Total:\s*(\d+)\]";
            var match = Regex.Match(chunk.Text, pattern);

            if (match.Success)
            {
                // 如果找到匹配的总分数，提取数字并添加到结果列表
                if (int.TryParse(match.Groups[1].Value, out int totalMark))
                {
                    // 如果当前行有 TextChunk，先将其添加到结果
                    if (currentLineChunks.Count > 0)
                    {
                        totals.Add((totalMark, new List<T>(currentLineChunks)));
                        currentLineChunks.Clear(); // 清空当前行的 TextChunk 列表
                    }
                    else
                    {
                        totals.Add((totalMark, new List<T> { chunk })); // 直接添加当前匹配的 TextChunk
                    }
                }
            }
            else
            {
                // 如果没有找到匹配，继续将当前 TextChunk 添加到当前行
                currentLineChunks.Add(chunk);
            }
        }

        // 处理最后一行的 TextChunk
        if (currentLineChunks.Count > 0)
        {
            totals.Add((0, new List<T>(currentLineChunks))); // 如果没有找到总分数，使用 0 作为占位符
        }

        return totals;
    }
}