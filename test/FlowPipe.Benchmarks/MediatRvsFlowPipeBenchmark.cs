using BenchmarkDotNet.Attributes;
using FlowPipe.Benchmarks.FlowPipe;
using FlowPipe.Benchmarks.MediatR;
using FlowPipe.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FlowPipe.Benchmarks;

[MemoryDiagnoser]
public class MediatRvsFlowPipeBenchmark
{
    private IServiceProvider _mediatrProvider;
    private IServiceProvider _flowPipeProvider;

    private IMessageDispatcher _flowPipeDispatcher;
    private ISender _sender;

    private PingRequestFlowPipe _requestFlowPipe = new() { UserId = 123 };
    private PingRequestMediatR _requestMediatR = new() { UserId = 123 };

    [GlobalSetup]
    public void Setup()
    {
        // MediatR için ServiceProvider kurulumu
        var mediatrServices = new ServiceCollection();
        mediatrServices.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(PingRequestMediatR).Assembly));
        _mediatrProvider = mediatrServices.BuildServiceProvider();
        _sender = _mediatrProvider.GetRequiredService<ISender>();

        // FlowPipe için ServiceProvider kurulumu
        var flowPipeServices = new ServiceCollection();
        flowPipeServices.AddFlowPipe(config => { config.AddAssembly(typeof(PingRequestFlowPipe).Assembly); });
        _flowPipeProvider = flowPipeServices.BuildServiceProvider();

        _flowPipeDispatcher = _flowPipeProvider.GetRequiredService<IMessageDispatcher>();
    }

    [Benchmark]
    public async Task<PingResponseMediatR> MediatR_Send()
    {
        return await _sender.Send(_requestMediatR);
    }

    [Benchmark]
    public async Task<PingResponseFlowPipe> FlowPipe_Send()
    {
        return await _flowPipeDispatcher.SendAsync(_requestFlowPipe);
    }
}