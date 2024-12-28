namespace PaSharper.Interfaces;

public interface IFilePairable<T>
{
    bool CanPairWith(T other);
}