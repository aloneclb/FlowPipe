using FlowPipe.Decorators;

namespace FlowPipe.UnitTests;

// public class TestBehavior<TIn, TOut> : IMessageBehavior<TIn, TOut> where TIn : IMessage<TOut>
// {
//     public int BehaviorSequence => 2;
//
//     public async Task<TOut> HandleAsync(TIn request, MessageHandlerDelegate<TOut> next, CancellationToken ct)
//     {
//         var response = await next();
//
//         if (request is Ping ping)
//         {
//             return response;
//         }
//
//         return default!;
//     }
// }

public class TestBehavior : IMessageBehavior<Ping, string>
{
    public int BehaviorSequence => 1;

    public Task<string> HandleAsync(Ping request, MessageHandlerDelegate<string> next, CancellationToken ct)
    {
        return Task.FromResult("Before -> " + next().Result + " -> After");
    }
}