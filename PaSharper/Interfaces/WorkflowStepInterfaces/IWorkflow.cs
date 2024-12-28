using PaSharper.Utilities.IO;

namespace PaSharper.Interfaces.WorkflowStepInterfaces;

public interface IWorkflow<TBaseData> where TBaseData: IFileMappable<TBaseData>
{
    void Execute(IEnumerable<MatchResult<TBaseData>> input);
}