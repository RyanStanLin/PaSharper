using ImageMagick;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using PaSharper.Builders;
using PaSharper.Data;
using PaSharper.Data.Base;
using PaSharper.Extensions;
using PaSharper.Interfaces.WorkflowStepInterfaces;
using PaSharper.Utilities.File;
using PaSharper.Utilities.IO;
using PaSharper.Utilities.WorkFlow.Implements.IGCSEs.Utilities;

namespace PaSharper.Utilities.WorkFlow.Implements.IGCSEs;

public class ImageExtractStep : IPipelineStep<PairResult<IgcseExamFileInfo>>
{
    private readonly string _libraryPath;
    /*
    private readonly MagickReadSettings _pdfDefaultReadingSettings = new MagickReadSettings
    {
        Density = new Density(300, 300),
        AntiAlias = true,
        TextAntiAlias = true,
        StrokeAntiAlias = true,
    };*/
    private readonly MagickReadSettings _pdfReadingSettings;
    private readonly uint _outputImageQuality;

    public ImageExtractStep(string libraryPath, MagickReadSettings pdfReadingSettings, uint outputImageQuality = 80)
    {
        if (string.IsNullOrEmpty(libraryPath))
            throw new ArgumentOutOfRangeException(nameof(libraryPath), "目录不可用");
        else
            _libraryPath = libraryPath;
        _pdfReadingSettings = pdfReadingSettings;
        _outputImageQuality = outputImageQuality;
    }

    public void Process(PairResult<IgcseExamFileInfo> input)
    {
        var builder = new QuestionLibraryBuilder("IGCSEs", "IG");
        foreach (var pairItem in input.PairedFiles)
        {
            foreach (var questionPaper in pairItem.Where(x => x.FileType == IgcsePaperType.Question))
            {
                string pdfPath = questionPaper.File.FullName;
                string pageText = String.Empty;
                int questionProcessStartPage = 2;
                int totalPageCount = 0;
                Rectangle pageSize;
                double topMarginY = 795;
                double bottomMarginY = 36;
                
                MagickImageCollection imageCollection = new MagickImageCollection();
                PdfTextExtractor extractor = new PdfTextExtractor();
                var pdfReader = new PdfReader(pdfPath);
                var pdfDocument = new PdfDocument(pdfReader);
                totalPageCount = pdfDocument.GetNumberOfPages();
                
                imageCollection.Read(questionPaper.File.FullName,_pdfReadingSettings);

                bool isUnfinishedQuestion = true;
                List<IMagickImage<ushort>> unfinishedQuestionImageCache = new List<IMagickImage<ushort>>();
                for (int pageIndex = questionProcessStartPage; pageIndex < totalPageCount; pageIndex++)
                { 
                    pageSize = pdfDocument.GetPage(pageIndex).GetPageSize();
                    
                    List<PdfTextExtractor.TextChunk> textChunks = extractor.ExtractTextWithPositions(pdfPath, pageIndex);
                    pageText = textChunks.Where(x => x.Rect.GetY() <= topMarginY && x.Rect.GetY() >= bottomMarginY )
                        .ExtractItems(x => x.Text)
                        .JoinToString();

                    textChunks.ProcessTextChunks((rowContext, chunks) =>
                    {
                        var totalMarks = TotalMatcher.FindTotals(chunks);
                    
                    if (totalMarks.Any())
                    {
                        double processedY = topMarginY;
                        foreach (var totalMarkData in totalMarks)
                        {
                            
                                double charY = totalMarkData.textChunks.First().Rect.GetY();
                                using (var cuttedImage = imageCollection.CutImage(pageIndex, processedY / pageSize.GetHeight(),
                                           charY / pageSize.GetHeight(), _outputImageQuality))
                                {
                                    unfinishedQuestionImageCache.Add(cuttedImage);
                                }
                                processedY += charY;
                                isUnfinishedQuestion = false;
                                builder.AddQuestion(new Question
                                {
                                    AISummary = "",
                                    QuestionImages = new List<QuestionImage>
                                    {
                                        (new QuestionImage
                                        {
                                            
                                        })
                                    }
                                    
                                }, "Math", 2024, "Spring", "Code1", "A");
                        }
                        isUnfinishedQuestion = false;
                    }
                    else if (isUnfinishedQuestion)
                    {
                        isUnfinishedQuestion = true;
                        using (var cuttedImage = imageCollection.CutImage(pageIndex, topMarginY / pageSize.GetHeight(),
                                   bottomMarginY / pageSize.GetHeight(), _outputImageQuality))
                        {
                            unfinishedQuestionImageCache.Add(cuttedImage);
                        }
                    }
                    });
                }
            }
        }
    }
}