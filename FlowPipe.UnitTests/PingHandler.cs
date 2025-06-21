using FlowPipe.Contracts;

namespace FlowPipe.UnitTests;

public class Ping : IMessage<string>;

public class PingHandler : IMessageHandler<Ping, string>
{
    public Task<string> HandleAsync(Ping message, CancellationToken cancellationToken)
        => Task.FromResult("Pong");
}