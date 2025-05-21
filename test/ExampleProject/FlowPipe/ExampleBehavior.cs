using ExampleProject.Feature;
using FlowPipe.Contracts;
using FlowPipe.Decorators;

namespace ExampleProject.FlowPipe;

public class ExampleBehavior<TIn, TOut> : IMessageBehavior<TIn, TOut> where TIn : IMessage<TOut>
{
    public int BehaviorSequence => 2;

    public async Task<TOut> HandleAsync(TIn request, MessageHandlerDelegate<TOut> next, CancellationToken ct)
    {
        if (request is PingRequest pingRequest)
        {
            Console.WriteLine($"ExampleBehavior {pingRequest.No}");
        }

        Console.WriteLine($"{typeof(ExampleBehavior<,>).Name} -> [BEFORE] Handling {typeof(TIn).Name}");
        var response = await next();
        Console.WriteLine($"{typeof(ExampleBehavior<,>).Name} -> [AFTER] Handled {typeof(TOut).Name}");

        if (response is PingResponse pingResponse)
        {
            Console.WriteLine($"ExampleBehavior {pingResponse.No}");
        }

        return response;
    }
}