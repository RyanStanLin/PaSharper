using PaSharper.Data.Base;
using PaSharper.Extensions;
using PaSharper.Interfaces.WorkflowStepInterfaces;
using PaSharper.Utilities.IO;

namespace PaSharper.Utilities.WorkFlow.Implements.IGCSEs;

public class FilePairingStep : IPipelineStep<IEnumerable<MatchResult<IgcseExamFileInfo>>, PairResult<IgcseExamFileInfo>>
{
    private readonly FilePairer<IgcseExamFileInfo> _filePairer;

    public FilePairingStep(FilePairer<IgcseExamFileInfo> filePairer)
    {
        _filePairer = filePairer;
    }

    public PairResult<IgcseExamFileInfo> Process(IEnumerable<MatchResult<IgcseExamFileInfo>> input)
    {
        return _filePairer.PairFiles(
            input.ExtractItems<MatchResult<IgcseExamFileInfo>, IgcseExamFileInfo>(x => x.ParsedData));
    }
}