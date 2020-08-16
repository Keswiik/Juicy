# Method Bindings
Useful when you need to inject a class that isn't correctly attributed for injections. This usually includes 3rd party dependencies.

```csharp
public sealed class Module : AbstractModule {
    [Provides]
    [Singleton]
    public DbConnection ProvideDatabaseConnection(
            [Named("connectionString")] string connectionString) {
        return new SqliteConnection(connectionString);
    }
}

...

public sealed class PersonServiceImpl {
    private DbConnection connection;

    [Inject]
    private PersonServiceImpl(DbConnection connection) {
        this.connection = connection;
    }

    public IPerson GetByFirstName(string firstname) {
        // use connection
    }
}
```

Bindings of this type are **only** detected when they are within a subclass of `AbstractModule`.