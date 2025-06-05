namespace FlowPipe.Contracts;

public interface IMessage;

public interface IMessage<TResponse> : IMessage
{
}