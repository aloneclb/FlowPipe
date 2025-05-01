using System.Reflection;

namespace FlowPipe.Models;

public class FlowPipeServiceConfiguration
{
    private List<Assembly> AssembliesToScan { get; } = new();
    public List<Assembly> GetAssemblies() => AssembliesToScan;
    
    public FlowPipeServiceConfiguration AddAssembly(Assembly assembly)
    {
        AssembliesToScan.Add(assembly);
        return this;
    }

    public FlowPipeServiceConfiguration AddAssemblies(params Assembly[] assemblies)
    {
        AssembliesToScan.AddRange(assemblies);
        return this;
    }
}