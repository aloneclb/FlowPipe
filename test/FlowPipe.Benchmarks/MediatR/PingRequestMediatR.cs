using MediatR;

namespace FlowPipe.Benchmarks.MediatR;

public record PingRequestMediatR : IRequest<PingResponseMediatR>
{
    public int UserId { get; set; }
}

public record PingResponseMediatR
{
    public required string ServerEndpoint { get; set; }
}

public class PingHandler : IRequestHandler<PingRequestMediatR, PingResponseMediatR>
{
    public async Task<PingResponseMediatR> Handle(PingRequestMediatR request, CancellationToken ct)
    {
        await Task.Run(() => { Console.WriteLine("Handler is work"); }, ct);

        return new PingResponseMediatR()
        {
            ServerEndpoint = $"www.google-{request.UserId}.com"
        };
    }
}