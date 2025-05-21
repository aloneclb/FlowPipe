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

        // Generic method invokes: SendInternal<TRequest, TResponse>
        var method = typeof(MessageDispatcher)
            .GetMethod(nameof(SendInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(requestType, typeof(TResponse));

        return (Task<TResponse>)method.Invoke(this, [request, ct])!;
    }

    private Task<TResponse> SendInternal<TRequest, TResponse>(TRequest request, CancellationToken ct)
        where TRequest : IMessage<TResponse>
    {
        var handler = serviceProvider.GetRequiredService<IMessageHandler<TRequest, TResponse>>();

        var behaviors = serviceProvider
            .GetServices<IMessageBehavior<TRequest, TResponse>>()
            .OrderBy(b => b.BehaviorSequence)
            .ToList();

        // handler metodumuz en içte olucak şekilde pipeline'ı oluşturuyoruz.
        MessageHandlerDelegate<TResponse> pipeline = () => handler.HandleAsync(request, ct);

        foreach (var behavior in behaviors.AsEnumerable().Reverse())
        {
            var next = pipeline;
            pipeline = () => behavior.HandleAsync(request, next, ct);
        }

        return pipeline();
    }
}