using Microsoft.Extensions.Logging;

namespace PaSharper.Extensions;

public static class IEnumerableExtensions
{
    /// <summary>
    /// Extracts specific properties of type <typeparamref name="TTarget"/> from a collection of type <typeparamref name="TSource"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the source collection.</typeparam>
    /// <typeparam name="TTarget">The type of the target properties to extract.</typeparam>
    /// <param name="source">The source collection to traverse.</param>
    /// <param name="extractor">A function to extract the target property from each source element.</param>
    /// <param name="loggerFactory">The factory for creating a logger instance.</param>
    /// <returns>An IEnumerable of extracted properties of type <typeparamref name="TTarget"/>.</returns>
    public static IEnumerable<TTarget> ExtractItems<TSource, TTarget>(
        this IEnumerable<TSource> source,
        Func<TSource, TTarget> extractor,
        ILoggerFactory loggerFactory)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (extractor == null) throw new ArgumentNullException(nameof(extractor));
        if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

        var logger = loggerFactory.CreateLogger(nameof(ExtractItems));
        
        foreach (var item in source)
        {
            TTarget result = default;
            try
            {
                result = extractor(item);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error extracting property from item: {item}");
            }
            if (result != null)
            {
                yield return result;
            }
        }
    }
}