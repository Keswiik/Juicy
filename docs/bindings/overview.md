## Binding Scopes

Currently, only two binding scopes are supported: `Instance` and `Singleton`.

When using any `Bind*` method within a modules `Configure` method, scope is set using `BindingBuilder.Scope(BindingScope scope)`.  
For bindings provided through methods, the `Scope` attribute can be used `[Scope(BindingScope scope)]`.

### Instance
Any bindings with an instanced scope will be created **every** time they are requested. Their values will never be cached.

You should _never_ have to explicitly state this scope.

### Singleton
Singleton bindings will be cached once they are requested for the first time. The cached value will be returned for any subsequent requests.

Certain bindings ([instance](./explicit.md#instance)) will default to this scope. While you can alter scope of these bindings explicitly, doing so is discouraged.

## Binding Names
Bindings can either be **named** or **unnamed**. Named bindings allow you to differentiate between bindings of the same type.

## Binding Conflicts
To avoid binding conflicts, you should follow two rules

* Only create one **unnamed** binding for a given type.
* Only create one binding the same name of a given type.

Failure to do so will cause an `InvalidOperationException` to be thrown when creating your injector with `Juicer.CreateInjector(...)`.

Here is an example of a module that breaks both of these rules:
```csharp
public sealed class BadModule : AbstractModule {
    public void Configure() {
        Bind<IService>()
            .To<ServiceImpl>()
            .In(BindingScopes.Singleton);
        Bind<int>()
            .ToInstance(8000)
            .Named("port");
    }

    [Provides]
    [Scope(BindingScope.Singleton)]
    IService ProvideService([Named("port")] port) {
        return new OtherServiceImpl(port);
    }

    [Provides]
    [Named("port")]
    [Scope(BindingScope.Singleton)]
    int ProvidePort(Configuration configuration) {
        return configuration.GetPort();
    }
}
```

## Lazily-Loaded Bindings
Any type that can be injected through Juicy can also be lazily-loaded by using injecting an `IProvider<T>` in place of the type you want.

These can be useful in situations where:

* Implementations do not have all required dependencies when they are first injected.
* Types reference eachother, so creation of one must be deferred until the other is created.
* Objects are expensive to create and are only injected when they are needed.

```csharp
Bind<IService>()
    .To<ServiceImpl>()
    .In(BindingScopes.Singleton);

...

public sealed class ServiceConsumer {
    private IProvider<IService> serviceProvider;

    [Inject]
    private ServiceConsumer(IProvider<IService> serviceProvider) {
        this.serviceProvider = serviceProvider;
    }

    public void Foo() {
        var service = serviceProvider.Get();
        // use service
    }
}
```