using iText.Kernel.Geom;

namespace PaSharper.Interfaces;

public interface ITextChunk<T>
{
    string Text { get; }
    Rectangle Rect { get; }
}