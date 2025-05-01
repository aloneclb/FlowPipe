using FlowPipe.Models;
using Microsoft.Extensions.DependencyInjection;

namespace FlowPipe.Extensions;

public static class FlowPipeServiceExtensions
{
    public static IServiceCollection AddFlowPipe(this IServiceCollection services)
    {
        services.AddScoped<IMessageDispatcher, MessageDispatcher>();
        return services;
    }

    public static IServiceCollection AddFlowPipe(this IServiceCollection services,
        Action<FlowPipeServiceConfiguration> configuration)
    {
        var serviceConfig = new FlowPipeServiceConfiguration();

        configuration.Invoke(serviceConfig);

        return services.AddFlowPipe(serviceConfig);
    }

    public static IServiceCollection AddFlowPipe(this IServiceCollection services, FlowPipeServiceConfiguration configuration)
    {
        // todo: servis config assembly scan vs.

        throw new NotImplementedException();
    }
}