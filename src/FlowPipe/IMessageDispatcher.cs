using FlowPipe.Contracts;

namespace FlowPipe;

public interface IMessageDispatcher
{
    Task<TResponse> SendAsync<TResponse>(IMessage<TResponse> request, CancellationToken ct = default);
}