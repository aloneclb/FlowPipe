# FlowPipe

Akış tabanlı mesaj dağıtımı ve işleyişi için basit ve esnek bir .NET Core kütüphanesi.

---

## Türkçe Kullanım Kılavuzu

### Giriş

FlowPipe, .NET Core projelerinde mesajlaşma (messaging) ve mesaj işleme (message handling) işlemlerini kolaylaştıran,
davranış zinciri (behavior pipeline) destekleyen bir yapı sunar. Örnek projede, HTTP isteklerini mesaj olarak ele alıp,
handler'lar ve davranışlar (behaviors) ile işlenmektedir.

### Kurulum ve Başlangıç

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFlowPipe(flowPipeConfig =>
{
    // Mesaj ve handler'lar bu assembly'de aranır
    // Birden fazla assembly vermek mümkün
    flowPipeConfig.AddAssembly(Assembly.GetExecutingAssembly()); 
});

var app = builder.Build();

app.Run();
```

### Mesaj Modeli

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

### Handler Modeli

```csharp
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
```

### Behavior Örneği

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

### Dispatcher Kullanım Örneği

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
