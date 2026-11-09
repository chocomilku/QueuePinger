namespace Cli.CommandRegistrar;

public interface ITypeResolver
{
    object? Resolve(Type? type);
}