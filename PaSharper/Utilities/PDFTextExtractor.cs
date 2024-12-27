using System;
using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace PaSharper.Tools;

public class PDFTextExtractor
{
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
        public List<TextChunk> ObjectResult { get; } = new List<TextChunk>();

        public override void EventOccurred(IEventData data, EventType type)
        {
            if (!type.Equals(EventType.RENDER_TEXT))
                return;

            TextRenderInfo renderInfo = (TextRenderInfo)data;

            string curFont = renderInfo.GetFont().GetFontProgram().ToString();
            float curFontSize = renderInfo.GetFontSize();

            IList<TextRenderInfo> text = renderInfo.GetCharacterRenderInfos();
            foreach (TextRenderInfo t in text)
            {
                string letter = t.GetText();
                Vector letterStart = t.GetBaseline().GetStartPoint();
                Vector letterEnd = t.GetAscentLine().GetEndPoint();
                Rectangle letterRect = new Rectangle(
                    letterStart.Get(0),
                    letterStart.Get(1),
                    letterEnd.Get(0) - letterStart.Get(0),
                    letterEnd.Get(1) - letterStart.Get(1)
                );
                if (letter == "\n")
                {
                    Console.WriteLine("return find!");
                }
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

    public List<TextChunk> ExtractTextWithPositions(string pdfPath, int pageNumber)
    {
        List<TextChunk> textChunks = new List<TextChunk>();

        using (PdfReader pdfReader = new PdfReader(pdfPath))
        using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
        {
            if (pageNumber < 1 || pageNumber > pdfDocument.GetNumberOfPages())
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number is out of range.");

            TextLocationStrategy strategy = new TextLocationStrategy();
            PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(pageNumber), strategy);

            textChunks.AddRange(strategy.ObjectResult);
        }

        return textChunks;
    }
    
   
}