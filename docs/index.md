## Configuring Your Bindings
All bindings for Juicy are configured by extending the `AbstractModule`.

Your bindings can be provided either through attributed methods, or explicitly by calling the `Bind<T>()`, `BindMany<T>()`, and `BindFactory<T>()` methods.

```csharp
public sealed class Module : AbstractModule {
    public void Configure() {
        Bind<IService>()
            .To<ServiceImpl>()
            .In(BindingScope.Singleton);
        Bind<int>()
            .ToInstance(5)
            .Named("toDouble");
        BindMany<List<ICommand>>()
            .To<InitializeCommand>()
            .To<PauseCommand>()
            .To<StartCommand>();
        BindFactory<IModelFactory>()
            .Implement<IModel, ModelImpl>();
    }

    [Provides]
    [Named("doubledNumber")]
    [Scope(BindingScope.Singleton)]
    public int ProvideDoubledNumber([Named("toDouble")] int five) {
        return five * 2;
    }
}
```

For more information about bindings, see [Bindings](./bindings/overview.md).

## Injecting Values

In order to make use of bindings, Juicy must know which constructors are "safe" to inject. To do this, you they will be marked with an `[Inject]` attribute.

A class can only have **ONE** injectable constructor. If an attributed constructor is not found, Juicy will look for a default constructor.

```csharp
public interface IService {
    int Handle(int number);
}

public sealed class ServiceImpl : IService {
    private readonly int number;

    [Inject]
    private ServiceImpl([Named("toDouble")] int number) {
        this.number = number;
    }

    public void Handle(int number) {
        return number * 2;
    }
}
```

## Creating an Injector

An injector is the entrypoint for _all_ injection in Juicy. Each injector can be provided with an array of modules to load bindings from.

An injector exposes two generic methods which are used to access all bindings. For unnamed bindings, `IInjector.Get<T>()`, while `IInjector.Get<T>(string)` is used for named bindings.

```csharp
public static class Program {
    private static void Main(string[] args) {
        var injector = Juicer.CreateInjector(new Module());
        var service = injector.Get<IService>();
        var number = injector.Get<int>("toDouble");
        var doubledNumber = injector.Get<int>("doubledNumber");

        Console.WriteLine(service.Handle(number) == doubledNumber);
    }
}
```

## Creating Child Injectors

Child injectors contain their own bindings, but will delegate to the injector that created them if a requested type has no bindings. This can be useful in situations where an implementation cannot be decided until after startup and requires additional dependencies to be installed.

```csharp
public sealed class Module : AbstractModule {
    [Provides]
    [Scope(BindingScope.Singleton)]
    DbConnection ProvideDbConnection(string connectionType) {
        IInjector injector;
        switch (connectionType) {
            case "sqlite":
                injector = CreateChildInjector(new SqliteModule());
                break;
            case "postgres":
                injector = CreateChildInjector(new PostgresModule());
                break;
            case "mysql":
                injector = CreateChildInjector(new MysqlModule());
                break;
            default:
                throw new InvalidOperationException("Could not determine DbConnection type.");
        }

        return injector.Get<DbConnection>();
    }
}
```

<sub><sub><sub>I know the examples are bad, but hopefully they get the idea across.</sub></sub></sub>

<sup><sup><sup><sup>Especially that last example.</sup></sup></sup></sup>