using System.Reflection;
using FlowPipe.Contracts;
using FlowPipe.Decorators;
using FlowPipe.Models;
using Microsoft.Extensions.DependencyInjection;

namespace FlowPipe.Extensions;

public static class FlowPipeServiceExtensions
{
    /// <summary>
    /// Adds the FlowPipe service with default configuration.
    /// </summary>
    public static IServiceCollection AddFlowPipe(this IServiceCollection services)
    {
        var serviceConfig = new FlowPipeServiceConfiguration();
        serviceConfig.AddAssembly(Assembly.GetExecutingAssembly());
        return services.InjectFlowPipe(serviceConfig);
    }

    /// <summary>
    /// Adds the FlowPipe service with a custom configuration.
    /// </summary>
    public static IServiceCollection AddFlowPipe(
        this IServiceCollection services,
        Action<FlowPipeServiceConfiguration> configuration)
    {
        var serviceConfig = new FlowPipeServiceConfiguration();
        configuration.Invoke(serviceConfig);
        return services.InjectFlowPipe(serviceConfig);
    }

    private static IServiceCollection InjectFlowPipe(
        this IServiceCollection services,
        FlowPipeServiceConfiguration configuration)
    {
        services.AddScoped<IMessageDispatcher, MessageDispatcher>();

        ValidateConfiguration(configuration);
        RegisterHandlersAndBehaviors(services, configuration.GetAssemblies);

        return services;
    }

    private static void ValidateConfiguration(FlowPipeServiceConfiguration configuration)
    {
        if (configuration.GetAssemblies.Count == 0)
        {
            throw new Exception("FlowPipe -> No assemblies have been added. Please add at least one assembly to scan.");
        }
    }

    private static void RegisterHandlersAndBehaviors(IServiceCollection services,
        IReadOnlyCollection<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .ToList();

            RegisterMessageHandlers(services, types);
            RegisterGenericBehaviors(services, types);
        }
    }

    private static void RegisterMessageHandlers(IServiceCollection services, IList<Type> types)
    {
        var handlerTypes = types
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<,>))
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var handler in handlerTypes)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }
    }

    private static void RegisterGenericBehaviors(IServiceCollection services, IList<Type> types)
    {
        var openGenericBehaviors = types
            .Where(t => t.IsGenericTypeDefinition)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IMessageBehavior<,>)));

        foreach (var openGenericBehavior in openGenericBehaviors)
        {
            var interfaceType = typeof(IMessageBehavior<,>);
            services.AddScoped(interfaceType, openGenericBehavior);
        }
    }
}