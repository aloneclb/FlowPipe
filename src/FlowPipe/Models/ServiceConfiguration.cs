using System.Reflection;

namespace FlowPipe.Models;

public class FlowPipeServiceConfiguration
{
    private List<Assembly> Assemblies { get; set; } = new();

    public void AddAssembly(Assembly assembly)
    {
        Assemblies.Add(assembly);
    }

    public void AddAssemblies(IEnumerable<Assembly> assemblies)
    {
        Assemblies.AddRange(assemblies);
    }

    public List<Assembly> GetAssemblies => Assemblies;
}