# FlowPipe

A simple and flexible .NET Core library for flow-based message dispatching and handling.

---

## English User Guide

### Introduction

FlowPipe provides a structure that simplifies messaging and message handling in .NET Core projects, with support for
behavior pipelines. In the sample project, HTTP requests are treated as messages and processed through handlers and
behaviors.

### Installation & Getting Started

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFlowPipe(flowPipeConfig =>
{
    // Messages and handlers will be discovered in this assembly
    // You can specify multiple assemblies
    flowPipeConfig.AddAssembly(Assembly.GetExecutingAssembly()); 
});

var app = builder.Build();

app.Run();
```

### Message Model

```csharp
public class PingRequest : IMessage<PingResponse>
{
    public int No { get; set; }
}
```

```csharp
public class PingResponse
{
    public int No { get; set; }
}
```

### Handler Model

```csharp
public class PingRequestHandler : IMessageHandler<PingRequest, PingResponse>
{
    public Task<PingResponse> HandleAsync(PingRequest request, CancellationToken ct = default)
    {
        Console.WriteLine($"Handler triggered {request.No}");

        return Task.FromResult(new PingResponse()
        {
            No = request.No + 1
        });
    }
}
```

### Behavior Example

```csharp
public class ExampleBehavior<TIn, TOut> : IMessageBehavior<TIn, TOut> where TIn : IMessage<TOut>
{
    public int BehaviorSequence => 2;

    public async Task<TOut> HandleAsync(TIn request, MessageHandlerDelegate<TOut> next, CancellationToken ct)
    {
        if (request is PingRequest pingRequest)
        {
            Console.WriteLine($"ExampleBehavior {pingRequest.No}");
        }

        Console.WriteLine($"{typeof(ExampleBehavior<,>).Name} -> [BEFORE] Handling {typeof(TIn).Name}");
        var response = await next();
        Console.WriteLine($"{typeof(ExampleBehavior<,>).Name} -> [AFTER] Handled {typeof(TOut).Name}");

        if (response is PingResponse pingResponse)
        {
            Console.WriteLine($"ExampleBehavior {pingResponse.No}");
        }

        return response;
    }
}
```

### Dispatcher Usage Example

```csharp
app.MapPost("/weatherforecast", async (
        [FromBody] PingRequest request,
        [FromServices] IMessageDispatcher dispatcher) =>
{
    var response = await dispatcher.SendAsync(request);
    return response;
})
.WithName("Ping Service");
```

### Dispatcher Usage Example 2 (Pipeline Ignoring)

```csharp
app.MapPost("/weatherforecast", async (
        [FromBody] PingRequest request,
        [FromServices] IMessageDispatcher dispatcher) =>
    {
        var response = await dispatcher.SendAsync(request, CancellationToken.None, false);
        return response;
    })
    .WithName("Ping Service");
```
