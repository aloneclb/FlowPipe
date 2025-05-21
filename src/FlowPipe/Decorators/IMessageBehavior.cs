using FlowPipe.Contracts;

namespace FlowPipe.Decorators;

public delegate Task<TResponse> MessageHandlerDelegate<TResponse>();

public interface IMessageBehavior<TRequest, TResponse> where TRequest : IMessage<TResponse>
{
    public int BehaviorSequence { get; }
    Task<TResponse> HandleAsync(TRequest request, MessageHandlerDelegate<TResponse> next, CancellationToken ct);
}