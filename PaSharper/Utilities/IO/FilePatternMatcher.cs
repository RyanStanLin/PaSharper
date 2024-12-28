using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using PaSharper.Interfaces;
using Microsoft.Extensions.Logging;

namespace PaSharper.Utilities.IO;

public class MatchResult<T>
{
    public T ParsedData { get; set; }
    public FileInfo FileDetails { get; set; }
    public bool IsPerfectMatch { get; set; }

    public MatchResult(T parsedData, FileInfo fileDetails, bool isPerfectMatch)
    {
        ParsedData = parsedData;
        FileDetails = fileDetails;
        IsPerfectMatch = isPerfectMatch;
    }
}

class FilePatternMatcher<T> where T : IFileMappable<T>
{
    private readonly string pattern;
    private readonly Func<Dictionary<string, string>, T> converter;
    private readonly List<(string keyword, int length, Action<T, object> mapper)> keywords;
    private readonly bool exactMatch;
    private readonly ILogger<FilePatternMatcher<T>> logger;

    public FilePatternMatcher(string inputFormat, T StructClass, bool exactMatch, ILoggerFactory loggerFactory)
    {
        var mappings = StructClass.GetMapping();
        this.keywords = ExtractKeywords(inputFormat, mappings);
        this.pattern = BuildRegexPattern(inputFormat, this.keywords, exactMatch);
        this.converter = CreateConverter(this.keywords);
        this.exactMatch = exactMatch;
        this.logger = loggerFactory.CreateLogger<FilePatternMatcher<T>>();
    }

    public IEnumerable<MatchResult<T>> MatchFiles(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"目录 {folderPath} 不存在");

        foreach (var filePath in Directory.EnumerateFiles(folderPath).Select(Path.GetFileName))
        {
            var fileInfo = new FileInfo($"{folderPath}/{filePath}");
            var match = Regex.Match(filePath, pattern);
            if (!match.Success) continue;

            var rawData = keywords
                .Select((k, i) => (k.keyword, match.Groups[i + 1].Value))
                .ToDictionary(kv => kv.keyword, kv => kv.Value);

            var obj = Activator.CreateInstance<T>();
            bool hasError = false;

            foreach (var (keyword, _, mapper) in keywords)
            {
                if (rawData.TryGetValue(keyword, out var value))
                {
                    try
                    {
                        mapper(obj, value);
                    }
                    catch (Exception ex)
                    {
                        logger.LogDebug($"Error when handling {keyword} 's value {value}: {ex.Message}, skipping this.");
                        hasError = true;
                    }
                }
            }

            yield return new MatchResult<T>(obj, fileInfo, !hasError);
        }
    }

    private List<(string keyword, int length, Action<T, object> mapper)> ExtractKeywords(
        string inputFormat,
        IEnumerable<(string keyword, Expression<Func<T, object>> property, Func<string, object> transformer)> mappings)
    {
        var mappingsDict = mappings.ToDictionary(m => m.keyword);

        return Regex.Matches(inputFormat, @"\{(\w+)(?::(\d+))?\}")
            .Cast<Match>()
            .Select<Match, (string keyword, int length, Action<T, object> mapper)>(m =>
            {
                var keyword = m.Groups[1].Value;
                if (!mappingsDict.ContainsKey(keyword))
                    throw new ArgumentException($"Keyword {keyword} was not been defined");

                var property = mappingsDict[keyword].property;
                var transformer = mappingsDict[keyword].transformer;
                var setter = CreateSetter(property);

                return (
                    keyword: keyword,
                    length: m.Groups[2].Success ? int.Parse(m.Groups[2].Value) : -1,
                    mapper: (obj, raw) =>
                    {
                        /*try
                        {*/
                            var value = transformer((string)raw);
                            setter(obj, value);
                        /*}
                        catch (Exception ex)
                        {*/
                            //logger.LogDebug($"Failed to convert: {keyword} -> {raw}. Error: {ex.Message}. Skipping this.");
                        //}
                    }
                );
            })
            .ToList();
    }

    private static string BuildRegexPattern(string inputFormat, IEnumerable<(string keyword, int length, Action<T, object> mapper)> keywords, bool exactMatch)
    {
        var regex = keywords.Aggregate(inputFormat, (current, k) =>
            current.Replace(
                k.length == -1 ? $"{{{k.keyword}}}" : $"{{{k.keyword}:{k.length}}}",
                k.length == -1 ? "(\\w+)" : $"([\\w]{{{k.length}}})"));

        return exactMatch ? $"^{regex}$" : regex;
    }

    private static Action<T, object> CreateSetter(Expression<Func<T, object>> property)
    {
        if (property.Body is MemberExpression member || 
            property.Body is UnaryExpression unary && (member = unary.Operand as MemberExpression) != null)
        {
            var propertyPath = new List<PropertyInfo>();
            while (member != null)
            {
                propertyPath.Insert(0, (PropertyInfo)member.Member);
                member = member.Expression as MemberExpression;
            }

            return (obj, value) =>
            {
                object currentObject = obj;
                for (int i = 0; i < propertyPath.Count - 1; i++)
                {
                    var propInfo = propertyPath[i];
                    var nextObject = propInfo.GetValue(currentObject);
                    if (nextObject == null)
                    {
                        nextObject = Activator.CreateInstance(propInfo.PropertyType);
                        propInfo.SetValue(currentObject, nextObject);
                    }
                    currentObject = nextObject;
                }

                var finalProperty = propertyPath[^1];
                finalProperty.SetValue(currentObject, Convert.ChangeType(value, finalProperty.PropertyType));
            };
        }

        throw new InvalidOperationException("The attribute mapping must be a valid attribute expression.");
    }

    private Func<Dictionary<string, string>, T> CreateConverter(IEnumerable<(string keyword, int length, Action<T, object> mapper)> keywordMappings)
    {
        return rawData =>
        {
            var instance = Activator.CreateInstance<T>();
            foreach (var (keyword, _, mapper) in keywordMappings)
            {
                if (rawData.TryGetValue(keyword, out var value))
                {
                    mapper(instance, value);
                }
            }
            return instance;
        };
    }
}