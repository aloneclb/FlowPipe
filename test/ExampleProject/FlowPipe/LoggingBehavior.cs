using FlowPipe.Contracts;
using FlowPipe.Decorators;

namespace ExampleProject.FlowPipe;

public class LoggingBehavior<TIn, TOut> : IMessageBehavior<TIn, TOut> where TIn : IMessage<TOut>
{
    public int BehaviorSequence => 1;

    public async Task<TOut> HandleAsync(TIn request, MessageHandlerDelegate<TOut> next, CancellationToken ct)
    {
        Console.WriteLine($"{typeof(LoggingBehavior<,>).Name} -> [BEFORE] Handling {typeof(TIn).Name}");
        var response = await next();
        Console.WriteLine($"{typeof(LoggingBehavior<,>).Name} -> [AFTER] Handled {typeof(TIn).Name}");
        return response;
    }
}