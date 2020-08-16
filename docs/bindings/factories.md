# Factory Bindings
Factory bindings make use of the `BindFactory<T>()` method within a module.

They allow you to create implementations of a type with consumer-provided values.

```csharp
public IConnectionFactory {
    IConnection Create(string url, int port);
}

public Connection : IConnection {
    private string url;
    private int port;
    private IConnectionConfiguration configuration;

    [Inject]
    private Connection(string url, int port, 
            IConnectionConfiguration configuration) {
        this.url = url;
        this.port = port;
        this.configuration = configuration;
    }
}
...
BindFactory<IConnectionFactory>()
    .Implement<IConnection, Connection>();
...

var factory = injector.Get<IConnectionFactory>();
var connection = factory.Create("/service", 8000);
```
In the above example, the fields `url` and `port` are passed from the factory to the new `Connection` instance. The `configuration` field is supplied by the injector.

## Named Parameters
If you have multiple parameters of the same type, it is necessary to name them, otherwise injection is not possible.
```csharp
public IPersonFactory {
    IPerson Create(
        [Name("firstname")] string firstname, 
        [Name("lastname")] string lastname,
        int age);
}

public Person : IPerson {
    private string firstname;
    private string lastname;
    private int age;

    [Inject]
    private Person(
            [Name("firstname")] string firstname, 
            [Name("lastname")] string lastname,
            int age) {
        this.firstname = firstname;
        this.lastname = lastname;
        this.age = age;
    }
}
...
BindFactory<IPersonFactory>()
    .Implement<IPerson, Person>();
...

var factory = injector.Get<IPersonFactory>();
var person = factory.Create("John", "Smith");
```

<sub><sub><sub>This example is really bad, but I was having trouble thinking of something to illustrate this.</sub></sub></sub>