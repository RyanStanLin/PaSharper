using PaSharper.Interfaces.WorkflowStepInterfaces;

namespace PaSharper.Utilities.WorkFlow;

public class PipelineExecutor
{
    private readonly List<IPipelineStep<object, object>> _steps = new();

    public void AddStep<TInput, TOutput>(IPipelineStep<TInput, TOutput> step)
    {
        // 将步骤添加到管道中
        _steps.Add(new StepWrapper<TInput, TOutput>(step));
    }

    public TOutput Execute<TInput, TOutput>(TInput input)
    {
        object current = input;

        foreach (var step in _steps)
        {
            current = step.Process(current);
        }

        return (TOutput)current;
    }

    private class StepWrapper<TInput, TOutput> : IPipelineStep<object, object>
    {
        private readonly IPipelineStep<TInput, TOutput> _step;

        public StepWrapper(IPipelineStep<TInput, TOutput> step)
        {
            _step = step;
        }

        public object Process(object input)
        {
            return _step.Process((TInput)input);
        }
    }
}