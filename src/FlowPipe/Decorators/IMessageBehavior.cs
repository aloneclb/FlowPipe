using FlowPipe.Contracts;

namespace FlowPipe.Decorators;

// public delegate Task<TResponse> MessageHandlerDelegate<TResponse>();
//
// public interface IMessageBehavior<TRequest, TResponse> where TRequest : IMessage<TResponse>
// {
//     public int BehaviorSequence { get; }
//     Task<TResponse> HandleAsync(TRequest request, MessageHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
// }
//
//
// public class LoggingBehavior<TRequest, TResponse> : IMessageBehavior<TRequest, TResponse>
//     where TRequest : IMessage<TResponse>
// {
//     public int BehaviorSequence => 0;
//     public async Task<TResponse> HandleAsync(TRequest request, MessageHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//     {
//         Console.WriteLine($"[BEFORE] Handling {typeof(TRequest).Name}");
//         var response = await next();
//         Console.WriteLine($"[AFTER] Handled {typeof(TRequest).Name}");
//         return response;
//     }
// }

public interface IMessageBehavior<TRequest, TResponse> where TRequest : IMessage<TResponse>
{
    public int BehaviorSequence { get; }
    Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken);
}

public class LoggingBehavior<TRequest, TResponse> : IMessageBehavior<TRequest, TResponse> where TRequest : IMessage<TResponse>
{
    public int BehaviorSequence => 0;

    public async Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[BEFORE] Handling {typeof(TRequest).Name}");
        var response = await next();
        Console.WriteLine($"[AFTER] Handled {typeof(TRequest).Name}");
        return response;
    }
}