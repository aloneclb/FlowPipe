namespace FlowPipe.Contracts;

public interface IMessageHandler<TRequest, TResponse> where TRequest : IMessage<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}