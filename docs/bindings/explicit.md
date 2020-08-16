# Explicit Bindings

Explicit bindings make use of the `Bind<T>()` method within a module.

## Type
Allows you to define what implementation of type you will use, but lets the injector create it for you.

```csharp
Bind<IService>()
    .To<ServiceImpl>()
    .In(BindingScopes.Singleton);

...

public sealed class ServiceConsumer {
    private IService service;

    [Inject]
    private ServiceConsumer(IService service) {
        this.service = service;
    }
}
```
Trying to use type bindings with value-types **WILL** result in errors.

## Instance
Bind to a specific instance of a type. This is primarily used when binding to primitives.

```csharp
Bind<int>()
    .ToInstance(8000)
    .Named("port");
Bind<string>()
    .ToInstance("/service")
    .Named("path");

...

public sealed class ServiceImpl : IService {
    private int port;
    private string path;

    [Inject]
    private ServiceImpl(
            [Named("port")] int port, 
            [Named("path")] string path) {
        this.port = port;
        this.path = path;
    }
}
```
Instance bindings **default** to the `Singleton` scope. While you can override scope, doing so **WILL** result in the injector trying to instantiate the types instead.