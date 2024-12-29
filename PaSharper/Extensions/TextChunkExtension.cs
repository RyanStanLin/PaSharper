using PaSharper.Interfaces;

namespace PaSharper.Extensions;

public static class TextChunkExtension
{
    public static IEnumerable<(string rowContext, List<T> chunks)> GroupTextChunksByLine<T>(this IEnumerable<T> textChunks) where T : ITextChunk<T>
    {
        if (textChunks == null || !textChunks.Any())
            yield break;

        // 按 Y 坐标排序，确保同一行的字符在一起
        var sortedChunks = textChunks.OrderBy(chunk => chunk.Rect.GetY()).ThenBy(chunk => chunk.Rect.GetX()).ToList();
        
        List<T> currentLine = new List<T>();
        double lastY = sortedChunks.First().Rect.GetY();

        foreach (var chunk in sortedChunks)
        {
            // 判断当前字符是否与上一个字符在同一行
            if (Math.Abs(chunk.Rect.GetY() - lastY) < 1e-5)
            {
                currentLine.Add(chunk);
            }
            else
            {
                // 如果不在同一行，保存当前行并开始新的一行
                var rowContext = currentLine.Select(x => x.Text).JoinToString();
                yield return (rowContext, currentLine);
                currentLine = new List<T> { chunk };
                lastY = chunk.Rect.GetY(); // 更新 lastY 为当前字符的 Y 坐标
            }
        }

        // 添加最后一行
        if (currentLine.Any())
        {
            var rowContext = currentLine.Select(x => x.Text).JoinToString();
            yield return (rowContext, currentLine);
        }
    }
    
    public static void ProcessTextChunks<T>(this IEnumerable<T> textChunks, Action<string, List<T>> processAction) where T : ITextChunk<T>
    {
        var groupedChunks = textChunks.GroupTextChunksByLine();

        foreach (var (rowContext, chunks) in groupedChunks)
        {
            // 调用传入的处理逻辑
            processAction(rowContext, chunks);
        }
    }
}