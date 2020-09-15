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
            .In(BindingScope.Singleton);
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
    .In(BindingScope.Singleton);

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

## Built-in Bindings
Built-in bindings are created automatically. They are included even if an injector is created with **no** modules.

Currently, this only includes the injector itself.

```csharp
public sealed class InjectorExample {
    private IInjector injector

    [Inject]
    private InjectorExample(IInjector injector) {
        this.injector = injector;
    }
}

...

var injectorExample = injector.Get<InjectorExample>();
```
`InjectorExample` now has access to the injector that was used to create it.

**Using the injector in this way should be avoided.**

## Binding Dependencies
Modules are able to install other modules. This can be used to decide binding implementations at runtime, or install external dependencies for your bindings.

```csharp
public sealed class ModuleWithDependencies : AbstractModule {
    public void Configure() {
        Install(new DependencyModule());
    }
}
```

## Overriding Bindings
Modules can override bindings from other modules, but **only** when the injector is being created or the modules are installed.

```csharp
public sealed class OverriddenModule : AbstractModule {
    public void Configure() {
        Bind<int>()
            .Named("Overridden")
            .ToInstance(5);
        Bind<int>()
            .Named("NotOverridden")
            .ToInstance(4);
    }
}

public sealed class BaseModule : AbstractModule {
    public void Configure() {
        Bind<int>()
            .Named("Overridden")
            .ToInstance(10);
    }
}

// installing the overridden module within another module

public sealed class Module : AbstractModule {
    public void Configure() {
        Install(new BaseModule().Override(new OverriddenModule()));
    }
}

// both base modules being used directly in an injector

Injector injector = Juicy.CreateInjector(new BaseModule().Override(new OverriddenModule()));

int overridden = injector.Get<int>("Overridden"); // 10
int notOverridden = injector.Get<int>("NotOverridden"); // 4
```

In the above example, the `"Overridden"` binding from the first module is replaced with the corresponding binding from `BaseModule`. Currently, this override logic also applies to **ALL** bindings installed in the module being overridden as well.