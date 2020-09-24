# Collection Bindings

Collection bindings make use of the `BindMany<T>()` or `BindDictionary<T>()` methods within a module. They involve creating one-to-many bindings.

## List / Set Bindings
List and set bindings make use of the `BindMany<T>()` method within a module. They allow you to bundle implementations of a service together.

```csharp
BindMany<List<ICommand>>()
    .To<InitializeCommand>()
    .To<PauseCommand>()
    .To<StartCommand>()
    .In(BindingScope.Singleton);
```
```csharp
public sealed class ServiceImpl {
    private List<ICommand> commands;

    [Inject]
    public ServiceImpl(List<ICommand> commands) {
        this.commands = commands;
    }
    ...
}
```

## Dictionary Bindings

Dictionary bindings make use of the `BindDictionary<T>()` method within a module. They allow you to map implementations of a service to keys.

```csharp
BindDictionary<Dictionary<string, ICommand>>()
    .To("StartCommand", typeof(StartCommand))
    .To("StopCommand", typeof(StopCommand))
    .In(BindingScope.Singleton);
```
```csharp
public sealed class ServiceImpl {
    private Dictionary<string, ICommand> commands;

    [Inject]
    public ServiceImpl(Dictionary<string, ICommand> commands) {
        this.commands = commands.
    }
    ...
}
```

## Using with Untargeted Bindings
[Untargeted Bindings](./explicit.md#untargeted) can be used in conjunction with collection bindings to determine how the individual implementing types are instantiated.

In the above example, the `List<ICommand>` is bound as a singleton, so the _entire_ list of values is cached after it is created. If you decided to inject a single command into another class, it would default to being instance-scoped.

```csharp
BindMany<List<ICommand>>()
    .To<InitializeCommand>()
    .To<PauseCommand>()
    .To<StartCommand>()
    .In(BindingScope.Singleton);
Bind<PauseCommand>()
    .In(BindingScope.Singleton);
```
The `PauseCommand` will now share its instance with the `List<ICommand>` binding, regardless of the order in which they are requested.

## Limitations
The type provided when calling `BindMany<T>()` must inherit from `IEnumerable` **AND** have an `Add(value)` method.

You **must** use the same type specified in the original `Bind...<T>()` call in order to have values injected.