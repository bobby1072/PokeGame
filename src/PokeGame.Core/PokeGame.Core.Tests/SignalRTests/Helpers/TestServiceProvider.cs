using Microsoft.Extensions.DependencyInjection;

namespace PokeGame.Core.Tests.SignalRTests.Helpers;

internal sealed class TestServiceProvider : IServiceProvider, IServiceScope
{
    private readonly Dictionary<Type, object> _services = new();

    public TestServiceProvider()
    {
        // Register self as IServiceScope
        _services[typeof(IServiceScope)] = this;
        _services[typeof(IServiceProvider)] = this;
    }

    public void RegisterService<T>(T service) where T : class
    {
        _services[typeof(T)] = service;
    }

    public object? GetService(Type serviceType)
    {
        _services.TryGetValue(serviceType, out var service);
        return service;
    }

    public T? GetService<T>()
    {
        return (T?)GetService(typeof(T));
    }

    public T GetRequiredService<T>() where T : notnull
    {
        var service = GetService<T>();
        if (service == null)
        {
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
        }
        return service;
    }

    public object GetRequiredService(Type serviceType)
    {
        var service = GetService(serviceType);
        if (service == null)
        {
            throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered.");
        }
        return service;
    }

    // IServiceScope implementation
    public IServiceProvider ServiceProvider => this;

    public void Dispose()
    {
        // No-op for testing
    }
}