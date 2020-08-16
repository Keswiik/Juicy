# Collection Bindings

Collection bindings make use of the `BindMany<T>()` method within a module. They allow you to bind multiple implementations of a service together.

```csharp
BindMany<List<ICommand>>()
    .To<InitializeCommand>()
    .To<PauseCommand>()
    .To<StartCommand>()
    .In(BindingScopes.Singleton);
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

## Limitations
The type provided when calling `BindMany<T>()` must inherit from `IEnumerable` **AND** have an `Add(value)` method.