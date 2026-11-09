namespace Console.CommandRegistrar;

public interface ITypeRegistrar
{
    ITypeResolver Build();
    void Register(Type service, Type implementation);
    void RegisterInstance(Type service, object implementation);
    void RegisterLazy(Type service, Func<object> factory);
}