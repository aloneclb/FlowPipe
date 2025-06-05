using FlowPipe.Contracts;

namespace FlowPipe.Benchmarks.FlowPipe;

public record PingRequestFlowPipe : IMessage<PingResponseFlowPipe>
{
    public int UserId { get; set; }
}

public record PingResponseFlowPipe
{
    public required string ServerEndpoint { get; set; }
}

public class PingHandler : IMessageHandler<PingRequestFlowPipe, PingResponseFlowPipe>
{
    public async Task<PingResponseFlowPipe> HandleAsync(PingRequestFlowPipe requestFlowPipe, CancellationToken ct = default)
    {
        await Task.Run(() => { Console.WriteLine("Handler is work"); }, ct);

        return new PingResponseFlowPipe()
        {
            ServerEndpoint = $"www.google-{requestFlowPipe.UserId}.com"
        };
    }
}