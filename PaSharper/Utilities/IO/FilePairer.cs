using PaSharper.Interfaces;

namespace PaSharper.Utilities.IO;

public class PairResult<T>
{
    public PairResult(IEnumerable<IEnumerable<T>> pairedFiles, IEnumerable<T> unmatchedFiles, bool isAllPerfectPaired)
    {
        PairedFiles = pairedFiles;
        UnmatchedFiles = unmatchedFiles;
        IsAllPerfectPaired = isAllPerfectPaired;
    }

    public IEnumerable<IEnumerable<T>> PairedFiles { get; set; }
    public IEnumerable<T> UnmatchedFiles { get; set; }
    public bool IsAllPerfectPaired { get; set; }
}

public class FilePairer<T> where T : IFilePairable<T>
{
    public PairResult<T> PairFiles(IEnumerable<T> files)
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

        foreach (var unmatchedFile in pairedFiles.Where(x => x.Count is 1).ToList())
        {
            unmatchedFiles.Add(unmatchedFile.First());
            pairedFiles.Remove(unmatchedFile);
        }

        return new PairResult<T>(pairedFiles, unmatchedFiles, !unmatchedFiles.Any());
    }
}