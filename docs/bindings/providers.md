# Providers
If method bindings within a module become lengthy, they can be moved to an implementation of the `Provider<T>` interface.

```csharp
public sealed class ProviderModule : AbstractModule {
    public void Configure() {
        Bind<IService>()
            .ToProvider<ServiceProvider>()
            .In(BindingScopes.Singleton);
    }
}

public sealed class ServiceProvider : IProvider<IService> {

    private readonly int timeout;

    [Inject]
    public ServiceProvider(IConfiguration configuration) {
        this.timeout = configuration.GetTimeout();
    }

    public IService Get() {
        return new ServiceImpl(timeout);
    }
}
```

While the above is a poor example, it illustrates how to set up bindings using an external provider.