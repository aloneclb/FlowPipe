using FlowPipe.Contracts;
using FlowPipe.Decorators;
using FlowPipe.ExampleAPI.Feature;

namespace FlowPipe.ExampleAPI.FlowPipe;

public class LoggingBehavior<TIn, TOut> : IMessageBehavior<TIn, TOut> where TIn : IMessage<TOut>
{
    public int BehaviorSequence => 1;

    public async Task<TOut> HandleAsync(TIn request, MessageHandlerDelegate<TOut> next, CancellationToken ct)
    {
        if (request is PingRequest pingRequest)
        {
            Console.WriteLine($"LoggingInput {pingRequest.No}");
        }

        Console.WriteLine($"{typeof(LoggingBehavior<,>).Name} -> [BEFORE] Handling {typeof(TIn).Name}");
        var response = await next();
        Console.WriteLine($"{typeof(ExampleBehavior<,>).Name} -> [AFTER] Handled {typeof(TOut).Name}");

        if (response is PingResponse pingResponse)
        {
            Console.WriteLine($"LoggingResponse {pingResponse.No}");
        }

        return response;
    }
}