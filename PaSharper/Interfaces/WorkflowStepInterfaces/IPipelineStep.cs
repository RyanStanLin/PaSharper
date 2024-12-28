namespace PaSharper.Interfaces.WorkflowStepInterfaces;

public interface IPipelineStep<TInput, TOutput>
{
    TOutput Process(TInput input);
}