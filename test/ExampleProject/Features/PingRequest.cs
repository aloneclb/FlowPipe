using FlowPipe.Contracts;

namespace ExampleProject.Features;

public record PingRequest : IMessage<PingResponse>
{
    public int UserId { get; set; }
}

public record PingResponse
{
    public required string ServerEndpoint { get; set; }
}

public class PingHandler : IMessageHandler<PingRequest, PingResponse>
{
    public async Task<PingResponse> HandleAsync(PingRequest request, CancellationToken ct = default)
    {
        await Task.Run(() => { Console.WriteLine("Handler is work"); }, ct);

        return new PingResponse()
        {
            ServerEndpoint = $"www.google-{request.UserId}.com"
        };
    }
}