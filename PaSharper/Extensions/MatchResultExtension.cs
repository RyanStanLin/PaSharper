using PaSharper.Utilities.IO;

namespace PaSharper.Extensions;

public static class MatchResultExtension
{
    public static IEnumerable<MatchResult<T>> ThenForEach<T>(this IEnumerable<MatchResult<T>> matchResults,
        Action<MatchResult<T>> action)
    {
        foreach (var matchResult in matchResults)
        {
            action(matchResult);
            yield return matchResult;
        }
    }
}