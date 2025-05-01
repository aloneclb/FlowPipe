using System.Reflection;
using FlowPipe.Contracts;
using FlowPipe.Decorators;
using FlowPipe.Models;
using Microsoft.Extensions.DependencyInjection;

namespace FlowPipe.Extensions;

public static class FlowPipeServiceExtensions
{
    public static IServiceCollection AddFlowPipe(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddFlowPipe(x => x.AddAssembly(assembly));

        return services;
    }

    public static IServiceCollection AddFlowPipe(this IServiceCollection services,
        Action<FlowPipeServiceConfiguration> flowPipeConfig)
    {
        var serviceConfig = new FlowPipeServiceConfiguration();
        flowPipeConfig.Invoke(serviceConfig);
        return services.AddFlowPipe(serviceConfig);
    }

    private static IServiceCollection AddFlowPipe(this IServiceCollection services, FlowPipeServiceConfiguration config)
    {
        services.AddScoped<IMessageDispatcher, MessageDispatcher>();

        foreach (var assembly in config.GetAssemblies().Distinct())
        {
            var types = assembly.GetTypes();

            foreach (var type in types.Where(t => t is { IsAbstract: false, IsInterface: false }))
            {
                var handlerInterfaces = type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<,>));

                // handler inject
                foreach (var iface in handlerInterfaces)
                {
                    services.AddScoped(iface, type);
                }

                // behavior inject
                if (type.IsGenericTypeDefinition && type.GetInterfaces().Any(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageBehavior<,>)))
                {
                    services.AddScoped(typeof(IMessageBehavior<,>), type);
                }
            }
        }

        return services;
    }
}