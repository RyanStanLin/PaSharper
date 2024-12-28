using System.Collections.Generic;
using System.Linq;
using PaSharper.Interfaces;

namespace PaSharper.Tools.IO;

public class FilePairer<T> where T : IFilePairable<T>
{
    public List<(T, T)> PairFiles(IEnumerable<T> files)
    {
        var pairedFiles = new List<(T, T)>();
        var unmatchedFiles = new List<T>(files);

        foreach (var file in files)
        {
            var match = unmatchedFiles.FirstOrDefault(f => f.CanPairWith(file));
            if (match != null)
            {
                pairedFiles.Add((file, match));
                unmatchedFiles.Remove(file);
                unmatchedFiles.Remove(match);
            }
        }

        return pairedFiles;
    }
}