using Microsoft.Extensions.Logging;
using PaSharper.Data;
using PaSharper.Extensions;
using PaSharper.Utilities.IO;

namespace PaSharper;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine(ConstantData.PASHARPER_TITLE);

        string folderPath = null;
        string inputFormat = null;
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        for (var i = 0; i < args.Length; i++)
            if (args[i] == "-p" && i + 1 < args.Length)
            {
                folderPath = args[i + 1];
                i++;
            }
            else if (args[i] == "-e" && i + 1 < args.Length)
            {
                inputFormat = args[i + 1];
                i++;
            }

        if (!string.IsNullOrEmpty(folderPath))
        {
            var matcher = new FilePatternMatcher<IgcseExamFileInfo>(
                inputFormat,
                new IgcseExamFileInfo(),
                false,
                loggerFactory
            );
            var results = matcher.MatchFiles(folderPath)
                .Where(x => x.IsPerfectMatch)
                .ThenForEach(result => result.ParsedData.File = result.FileDetails);
            foreach (var result in results)
                Console.WriteLine(
                    $"\n[{result.ParsedData.FileType.ToCustomString()}]{result.ParsedData.Subject.SubjectID}-{result.ParsedData.ExamTime.Year}{result.ParsedData.ExamTime.Season.ToCustomString()}-" +
                    $"{result.ParsedData.PaperNumber}{result.ParsedData.PaperVersion}卷:");
            //igPairer.PairFiles(result);
            /*PDFTextExtractor extractor = new PDFTextExtractor();
                List<PDFTextExtractor.TextChunk> textChunks = extractor.ExtractTextWithPositions(result.FileDetails.FullName, 1);

                foreach (var textChunk in textChunks.Where(x=>x.Rect.GetY()<=795 && x.Rect.GetY()>=35 ))
                {
                    Console.Write(textChunk.Text);
                }*/
            FilePairer<IgcseExamFileInfo> igPairer = new();
            var pairFiles =
                igPairer.PairFiles(
                    results.ExtractItems<MatchResult<IgcseExamFileInfo>, IgcseExamFileInfo>(x => x.ParsedData,
                        loggerFactory));
            Console.WriteLine(pairFiles.pairedFiles.Count()+3);
        }
    }
}