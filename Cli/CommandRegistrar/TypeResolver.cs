using Spectre.Console.Cli;

namespace Cli.CommandRegistrar;

public sealed class TypeResolver(IServiceProvider provider) : ITypeResolver
{
    public object? Resolve(Type? type)
    {
        return type == null ? null : provider.GetService(type);
    }
}