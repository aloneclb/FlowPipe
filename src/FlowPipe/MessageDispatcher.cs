using System.Reflection;
using FlowPipe.Contracts;
using FlowPipe.Decorators;
using Microsoft.Extensions.DependencyInjection;

namespace FlowPipe;

public class MessageDispatcher(IServiceProvider serviceProvider) : IMessageDispatcher
{
    public Task<TResponse> SendAsync<TResponse>(IMessage<TResponse> request, CancellationToken ct = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IMessageHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = serviceProvider.GetRequiredService(handlerType);

        var behaviors = serviceProvider
            .GetServices(typeof(IMessageBehavior<,>).MakeGenericType(requestType, typeof(TResponse)))
            .Cast<dynamic>()
            .OrderBy(x => x.BehaviorSequence)
            .ToList();

        // En içte handler olacak şekilde pipeline'ı oluştur
        Func<Task<TResponse>> pipeline = () => ((dynamic)handler).HandleAsync((dynamic)request, ct);

        foreach (dynamic behavior in behaviors)
        {
            Func<Task<TResponse>> next = pipeline;
            pipeline = () => behavior.HandleAsync((dynamic)request, ct, next);
        }

        return pipeline();
    }
}


// public class MessageDispatcher(IServiceProvider serviceProvider) : IMessageDispatcher
// {
//     public Task<TResponse> SendAsync<TResponse>(IMessage<TResponse> request, CancellationToken ct = default)
//     {
//         var requestType = request.GetType();
//
//         // Generic method invoke: SendInternal<TRequest, TResponse>
//         var method = typeof(MessageDispatcher)
//             .GetMethod(nameof(SendInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
//             .MakeGenericMethod(requestType, typeof(TResponse));
//
//         return (Task<TResponse>)method.Invoke(this, [request, ct])!;
//     }
//
//     private Task<TResponse> SendInternal<TRequest, TResponse>(TRequest request, CancellationToken ct)
//         where TRequest : IMessage<TResponse>
//     {
//         var handler = serviceProvider.GetRequiredService<IMessageHandler<TRequest, TResponse>>();
//
//         var behaviors = serviceProvider
//             .GetServices<IMessageBehavior<TRequest, TResponse>>()
//             .OrderBy(b => b.BehaviorSequence)
//             .ToList();
//
//         Func<Task<TResponse>> pipeline = () => handler.HandleAsync(request, ct);
//
//         foreach (var behavior in behaviors.AsEnumerable().Reverse())
//         {
//             var next = pipeline;
//             pipeline = () => behavior.HandleAsync(request, next, ct);
//         }
//
//         return pipeline();
//     }
// }
