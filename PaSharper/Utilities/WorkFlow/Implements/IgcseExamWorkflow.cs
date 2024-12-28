using System.Collections;
using PaSharper.Data;
using PaSharper.Data.Base;
using PaSharper.Interfaces.WorkflowStepInterfaces;
using PaSharper.Utilities.IO;
using PaSharper.Utilities.WorkFlow.Implements.IGCSEs;

namespace PaSharper.Utilities.WorkFlow.Implements;

public class IgcseExamWorkflow: IWorkflow<IgcseExamFileInfo>
{
    private readonly PipelineExecutor _pipelineExecutor;

    public IgcseExamWorkflow()
    {
        _pipelineExecutor = new PipelineExecutor();
        InitializeSteps();
    }

    private void InitializeSteps()
    {
        _pipelineExecutor.AddStep(new FilePairingStep(new FilePairer<IgcseExamFileInfo>()));
        //_pipelineExecutor.AddStep(new ImageProcessingStep(new IgcseImageProcessor()));
        //_pipelineExecutor.AddStep(new DatabaseBuildingStep(new IgcseQuestionDatabaseBuilder()));
    }

    public void Execute(IEnumerable<MatchResult<IgcseExamFileInfo>> input)
    {
        _pipelineExecutor.Execute<IEnumerable<MatchResult<IgcseExamFileInfo>>,object>(input);
    }
}