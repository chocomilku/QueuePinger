namespace Console.CommandRegistrar;

public interface ITypeResolver
{
    object? Resolve(Type? type);
}