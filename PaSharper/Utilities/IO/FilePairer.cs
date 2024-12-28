using PaSharper.Interfaces;

namespace PaSharper.Utilities.IO;

public class FilePairer<T> where T : IFilePairable<T>
{
    public List<List<T>> PairFiles(IEnumerable<T> files)
    {
        var pairedFiles = new List<List<T>>();
        var unmatchedFiles = new List<T>(files);

        while (unmatchedFiles.Any())
        {
            var currentFile = unmatchedFiles.First();
            var group = new List<T> { currentFile };
            unmatchedFiles.Remove(currentFile);

            var matches = unmatchedFiles.Where(f => f.CanPairWith(currentFile)).ToList();
            foreach (var match in matches)
            {
                group.Add(match);
                unmatchedFiles.Remove(match);
            }

            pairedFiles.Add(group);
        }

        return pairedFiles;
    }
}