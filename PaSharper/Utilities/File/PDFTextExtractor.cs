using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace PaSharper.Utilities.File;

public class PDFTextExtractor
{
    public List<TextChunk> ExtractTextWithPositions(string pdfPath, int pageNumber)
    {
        List<TextChunk> textChunks = new();

        using (var pdfReader = new PdfReader(pdfPath))
        using (var pdfDocument = new PdfDocument(pdfReader))
        {
            if (pageNumber < 1 || pageNumber > pdfDocument.GetNumberOfPages())
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number is out of range.");

            var strategy = new TextLocationStrategy();
            PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(pageNumber), strategy);

            textChunks.AddRange(strategy.ObjectResult);
        }

        return textChunks;
    }

    public class TextChunk
    {
        public string Text { get; set; }
        public Rectangle Rect { get; set; }
        public string FontFamily { get; set; }
        public float FontSize { get; set; }
        public float SpaceWidth { get; set; }
    }

    private class TextLocationStrategy : LocationTextExtractionStrategy
    {
        public List<TextChunk> ObjectResult { get; } = new();

        public override void EventOccurred(IEventData data, EventType type)
        {
            if (!type.Equals(EventType.RENDER_TEXT))
                return;

            var renderInfo = (TextRenderInfo)data;

            var curFont = renderInfo.GetFont().GetFontProgram().ToString();
            var curFontSize = renderInfo.GetFontSize();

            IList<TextRenderInfo> text = renderInfo.GetCharacterRenderInfos();
            foreach (var t in text)
            {
                var letter = t.GetText();
                var letterStart = t.GetBaseline().GetStartPoint();
                var letterEnd = t.GetAscentLine().GetEndPoint();
                var letterRect = new Rectangle(
                    letterStart.Get(0),
                    letterStart.Get(1),
                    letterEnd.Get(0) - letterStart.Get(0),
                    letterEnd.Get(1) - letterStart.Get(1)
                );
                if (letter == "\n") Console.WriteLine("return find!");
                ObjectResult.Add(new TextChunk
                {
                    Text = letter,
                    Rect = letterRect,
                    FontFamily = curFont,
                    FontSize = curFontSize,
                    SpaceWidth = t.GetSingleSpaceWidth() / 2f
                });
            }
        }
    }
}