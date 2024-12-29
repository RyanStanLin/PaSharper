using PaSharper.Interfaces.WorkflowStepInterfaces;

namespace PaSharper.Utilities.WorkFlow;

public class PipelineExecutor
{
    private readonly List<object> _steps = new();

    public void AddStep<TInput, TOutput>(IPipelineStep<TInput, TOutput> step)
    {
        _steps.Add(new StepWrapper<TInput, TOutput>(step));
    }

    public void AddStep<TInput>(IPipelineStep<TInput> step)
    {
        _steps.Add(new StepWrapper<TInput>(step));
    }

    public TOutput Execute<TInput, TOutput>(TInput input)
    {
        object current = input;

        foreach (var step in _steps)
        {
            if (step is IStepWrapper<TInput, TOutput> wrappedStep)
            {
                current = wrappedStep.Process(current);
            }
            else if (step is IStepWrapper<TInput> voidStep)
            {
                voidStep.Process(current);
            }
        }

        return (TOutput)current;
    }

    private interface IStepWrapper<TInput, TOutput>
    {
        TOutput Process(object input);
    }

    private interface IStepWrapper<TInput>
    {
        void Process(object input);
    }

    private class StepWrapper<TInput, TOutput> : IStepWrapper<TInput, TOutput>
    {
        private readonly IPipelineStep<TInput, TOutput> _step;

        public StepWrapper(IPipelineStep<TInput, TOutput> step)
        {
            _step = step;
        }

        public TOutput Process(object input)
        {
            return _step.Process((TInput)input);
        }
    }

    private class StepWrapper<TInput> : IStepWrapper<TInput>
    {
        private readonly IPipelineStep<TInput> _step;

        public StepWrapper(IPipelineStep<TInput> step)
        {
            _step = step;
        }

        public void Process(object input)
        {
            _step.Process((TInput)input);
        }
    }
}