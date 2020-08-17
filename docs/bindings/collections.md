# Collection Bindings

Collection bindings make use of the `BindMany<T>()` method within a module. They allow you to bind multiple implementations of a service together.

```csharp
BindMany<List<ICommand>>()
    .To<InitializeCommand>()
    .To<PauseCommand>()
    .To<StartCommand>()
    .In(BindingScope.Singleton);
```
You must use the same type specified in the original `BindMany<T>()` call in order to have the values injected.

```csharp
public sealed class ServiceImpl {
    private List<ICommand> commands;

    private ServiceImpl(List<ICommand> commands) {
        this.commands = commands;
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