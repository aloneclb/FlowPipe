using System.Reflection;
using FlowPipe.Contracts;
using FlowPipe.Decorators;
using Microsoft.Extensions.DependencyInjection;

namespace FlowPipe;

public class MessageDispatcher(IServiceProvider serviceProvider) : IMessageDispatcher
{
    public Task<TResponse> SendAsync<TResponse>(IMessage<TResponse> request, CancellationToken ct = default,
        bool pipelineActive = true)
    {
        Type requestType = request.GetType();

        // Generic method invokes: SendInternal<TRequest, TResponse>
        MethodInfo method = typeof(MessageDispatcher)
            .GetMethod(nameof(SendInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(requestType, typeof(TResponse));

        return (Task<TResponse>)method.Invoke(this, [request, pipelineActive, ct])!;
    }

    private Task<TResponse> SendInternal<TRequest, TResponse>(TRequest request, bool pipelineActive, CancellationToken ct)
        where TRequest : IMessage<TResponse>
    {
        var handler = serviceProvider.GetRequiredService<IMessageHandler<TRequest, TResponse>>();
        
        // handler metodumuz en içte olucak şekilde pipeline'ı oluşturuyoruz.
        MessageHandlerDelegate<TResponse> pipeline = () => handler.HandleAsync(request, ct);

        if (pipelineActive)
        {
            var behaviors = serviceProvider
                .GetServices<IMessageBehavior<TRequest, TResponse>>()
                .OrderBy(b => b.BehaviorSequence)
                .ToList();

            foreach (var behavior in behaviors.AsEnumerable().Reverse())
            {
                var next = pipeline;
                pipeline = () => behavior.HandleAsync(request, next, ct);
            }
        }

        return pipeline();
    }
}

// Expression tree maybe after 
// public class MessageDispatcher(IServiceProvider serviceProvider) : IMessageDispatcher
// {
//     private static readonly
//         ConcurrentDictionary<(Type RequestType, Type ResponseType),
//             Func<IServiceProvider, object, CancellationToken, Task<object>>> DispatchCache = new();
//
//     public async Task<TResponse> SendAsync<TResponse>(IMessage<TResponse> request, CancellationToken ct = default)
//     {
//         var key = (request.GetType(), typeof(TResponse));
//
//         var func = DispatchCache.GetOrAdd(key, static key =>
//         {
//             var methodInfo = typeof(MessageDispatcher)
//                 .GetMethod(nameof(InvokeInternalGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
//                 .MakeGenericMethod(key.RequestType, key.ResponseType);
//
//             var serviceProviderParam = Expression.Parameter(typeof(IServiceProvider), "sp");
//             var requestParam = Expression.Parameter(typeof(object), "request");
//             var ctParam = Expression.Parameter(typeof(CancellationToken), "ct");
//
//             var call = Expression.Call(methodInfo, serviceProviderParam, requestParam, ctParam);
//
//             var lambda = Expression.Lambda<Func<IServiceProvider, object, CancellationToken, Task<object>>>(
//                 call, serviceProviderParam, requestParam, ctParam
//             );
//
//             return lambda.Compile();
//         });
//
//         var result = await func(serviceProvider, request, ct);
//         return (TResponse)result;
//     }
//
//     private static async Task<object> InvokeInternalGeneric<TRequest, TResponse>(IServiceProvider sp, object requestObj,
//         CancellationToken ct)
//         where TRequest : IMessage<TResponse>
//     {
//         var dispatcher = new MessageDispatcher(sp);
//         var result = await dispatcher.SendInternal<TRequest, TResponse>((TRequest)requestObj, ct);
//         return result!;
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
//         MessageHandlerDelegate<TResponse> pipeline = () => handler.HandleAsync(request, ct);
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