using iText.Kernel.Geom;

namespace PaSharper.Data;

public static class PdfTextExtrator
{
    [Obsolete]
    public class TextChunk
    {
        public string Text { get; set; }
        public Rectangle Rect { get; set; }
        public string FontFamily { get; set; }
        public float FontSize { get; set; }
        public float SpaceWidth { get; set; }
    }
}