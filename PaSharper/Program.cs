using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using PaSharper.Data;
using PaSharper.Tools;

namespace PaSharper;

class Program
{
    static void Main(string[] args)
    {
        TitlePrinter.PrintTitle();
        
        string folderPath = null;
        string inputFormat = null;
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        for (int i = 0; i < args.Length; i++)
        {
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
        }

        if (!string.IsNullOrEmpty(folderPath))
        {
            var matcher = new FilePatternMatcher<IgcseExamFileInfo>(
                inputFormat,
                new IgcseExamFileInfo(),
                exactMatch: false,
                loggerFactory
            );
            foreach (var result in matcher.MatchFiles(folderPath).Where(x=> x.IsPerfectMatch == true))
            {
                Console.WriteLine($"\n[{result.ParsedData.FileType.ToCustomString()}]{result.ParsedData.Subject.SubjectID}-{result.ParsedData.ExamTime.Year}{result.ParsedData.ExamTime.Season.ToCustomString()}-" +
                                  $"{result.ParsedData.PaperNumber}{result.ParsedData.PaperVersion}卷:");
                /*PDFTextExtractor extractor = new PDFTextExtractor();
                List<PDFTextExtractor.TextChunk> textChunks = extractor.ExtractTextWithPositions(result.FileDetails.FullName, 1);

                foreach (var textChunk in textChunks.Where(x=>x.Rect.GetY()<=795 && x.Rect.GetY()>=35 ))
                {
                    Console.Write(textChunk.Text);
                }*/
            }
        }
    }
}