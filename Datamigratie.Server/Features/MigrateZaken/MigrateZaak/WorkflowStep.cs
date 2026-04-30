namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak;

/// <summary>
/// Executes an async step and produces an output of type <typeparamref name="TOut"/>.
/// </summary>
public delegate Task<TOut> Run<TIn, TOut>(TIn input);

/// <summary>
/// Rolls back the side effects of a step, given the original input and the output it produced.
/// </summary>
public delegate Task Compensate<TIn, TOut>(TIn input, TOut output);

/// <summary>
/// A single step in a saga-style workflow with automatic compensation on failure.
///
/// <para>
/// Steps are chained with <see cref="WorkflowStep.Then{TIn,TOut}"/> to form a pipeline.
/// When a later step fails, all preceding steps are compensated in reverse order.
/// </para>
///
/// <para><b>Exception behaviour:</b></para>
/// <list type="bullet">
///   <item>If a step fails and compensation succeeds, the original exception is re-thrown as-is.</item>
///   <item>If a step fails and compensation also fails, both exceptions are combined in an <see cref="AggregateException"/>.</item>
///   <item>If the first step in the chain fails, no compensation runs (nothing to undo) and the exception propagates directly.</item>
/// </list>
/// </summary>
/// <typeparam name="TIn">The type of input the workflow accepts.</typeparam>
/// <typeparam name="TOut">The type of output this step produces.</typeparam>
public class WorkflowStep<TIn, TOut>(Run<TIn, TOut> run, Compensate<TIn, TOut>? compensate = null)
{
    private TIn? _input;
    private TOut? _output;
    private bool _ran;

    /// <summary>
    /// Executes the step with the given <paramref name="input"/>, storing the result for potential compensation.
    /// </summary>
    public async Task<TOut> Run(TIn input)
    {
        _input = input;
        _output = await run(input);
        _ran = true;
        return _output;
    }

    /// <summary>
    /// Rolls back this step if it was executed and a compensate delegate was provided.
    /// Does nothing if the step never ran or has no compensate delegate.
    /// </summary>
    public async Task Compensate()
    {
        if (compensate != null && _ran)
        {
            await compensate(_input!, _output!);
        }
    }
}

/// <summary>
/// Extension methods for chaining <see cref="WorkflowStep{TIn,TOut}"/> instances into a saga workflow.
/// </summary>
public static class WorkflowStep
{
    /// <summary>
    /// Appends a void action that receives the previous step's output, executes it, and passes the output through unchanged.
    /// Use this for side-effect-only steps (e.g. linking, unlocking) that don't transform the data.
    /// </summary>
    public static WorkflowStep<TIn, TOut> Then<TIn, TOut>(
        this WorkflowStep<TIn, TOut> step, Func<TOut, Task> action,
        Compensate<TIn, TOut>? compensate = null) => step.Then
        (
            async input =>
            {
                await action(input);
                return input;
            },
            compensate
        );

    /// <summary>
    /// Appends a transforming step that maps the previous output to a new type.
    /// <para>
    /// If <paramref name="transform"/> throws, the preceding step chain is compensated in reverse.
    /// If compensation succeeds, the original exception is re-thrown.
    /// If compensation also fails, an <see cref="AggregateException"/> containing both is thrown.
    /// </para>
    /// </summary>
    public static WorkflowStep<TIn, TNewOut> Then<TIn, TOut, TNewOut>(
        this WorkflowStep<TIn, TOut> step,
        Run<TOut, TNewOut> transform,
        Compensate<TIn, TNewOut>? compensate = null)
    {
        return new WorkflowStep<TIn, TNewOut>(
            run: async input =>
            {
                var output = await step.Run(input);
                try
                {
                    return await transform(output);
                }
                catch (System.Exception ex1)
                {
                    try
                    {
                        await step.Compensate();
                    }
                    catch (Exception ex2)
                    {
                        throw new AggregateException(ex1, ex2).Flatten();
                    }
                    throw;
                }
            },
            // Compensate in reverse: first undo this step, then undo the preceding step.
            // Failures are collected so both compensations are always attempted.
            compensate: async (input, newOutput) =>
            {
                List<Exception> exceptions = [];
                try
                {
                    if (compensate != null)
                    {
                        await compensate(input, newOutput);
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
                try
                {
                    await step.Compensate();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
                if (exceptions.Count > 0)
                {
                    throw new AggregateException(exceptions).Flatten();
                }
            }
        );
    }
}
