using FlowPipe.Contracts;
using FlowPipe.Decorators;

namespace ExampleProject.FlowPipe;

public class ValidatorBehavior<TIn, TOut> : IMessageBehavior<TIn, TOut> where TIn : IMessage<TOut>
{
    public int BehaviorSequence => 2;

    public async Task<TOut> HandleAsync(TIn request, Func<Task<TOut>> next, CancellationToken ct)
    {
        Console.WriteLine($"{typeof(ValidatorBehavior<,>).Name} -> [BEFORE] Handling {typeof(TIn).Name}");
        var response = await next();
        Console.WriteLine($"{typeof(ValidatorBehavior<,>).Name} -> [AFTER] Handled {response?.GetType().Name}");
        return response;
    }
}