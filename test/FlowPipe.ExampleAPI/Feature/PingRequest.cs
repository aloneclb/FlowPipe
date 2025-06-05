using FlowPipe.Contracts;

namespace FlowPipe.ExampleAPI.Feature;

public class PingRequest : IMessage<PingResponse>
{
    public int No { get; set; }
}

public class PingResponse
{
    public int No { get; set; }
}

public class PingRequestHandler : IMessageHandler<PingRequest, PingResponse>
{
    public Task<PingResponse> HandleAsync(PingRequest request, CancellationToken ct = default)
    {
        Console.WriteLine($"Handler tetiklendi {request.No}");

        return Task.FromResult(new PingResponse()
        {
            No = request.No + 1
        });
    }
}