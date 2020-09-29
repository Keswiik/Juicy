# Logging
Juicy supports logging through the [Microsoft Logging Abstractions](Microsoft.Extensions.Logging.Abstractions).
In order for logs to appear, an `ILoggingFactory` must be provider to the `Juicer` prior to creating an injector.

```csharp
Juicer.AddLogging(new LoggerFactory());
Juicer.CreateInjector(new Module());
```

**All** logging in Juicy is done at the Trace/Verbose level. **No** exceptions will ever be logged; they will always bubble back up to their corresponding `Get()` call or factory method invocation.

## Serilog
To add logging support through Serilog, add the `Serilog.Extensions.Logging` package, then whatever combination of configuration and sinks you desire.

For our example, we will be using the following Serilog and Microsoft packages to enable configuration through `appsettings.json`:

- `Serilog.Settings.Configuration`
- `Serilog.Sinks.Console`
- `Serilog.Sinks.File`
- `Microsoft.Extensions.Configuration.Json`

```csharp
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

Juicer.AddLogging(new SerilogLoggerFactory(logger: logger));
```

The above will create loggers that read their configuration from `./appsettings.json`.

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default":  "Verbose"
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": { "path": "Logs/log.txt" }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": {
      "Application": "Sample"
    }
  }
}
```

This configuration will send logs both the console and a `log.txt` file.

What follows is example output that would be seen when an injector is processing bindings at startup.

```
2020-09-29 12:12:46.778 -05:00 [VRB] Processing bindings for module of type OverrideModule
2020-09-29 12:12:46.968 -05:00 [VRB] Resolved binding MethodBinding[name=PrintString scope=Singleton baseType=String method=ProvidePrintString]
2020-09-29 12:12:46.970 -05:00 [VRB] Resolved binding MethodBinding[name=DoubledNum1 scope=Singleton baseType=Int32 method=ProvideDoubledNum1]
2020-09-29 12:12:46.971 -05:00 [VRB] Resolved binding ConcreteBinding[name= scope=Singleton baseType=IService implementation=ServiceImpl]
2020-09-29 12:12:46.971 -05:00 [VRB] Resolved binding ConcreteBinding[name= scope=Singleton baseType=ServiceImpl implementation=ServiceImpl]
2020-09-29 12:12:46.972 -05:00 [VRB] Resolved binding ConcreteBinding[name= scope=Singleton baseType=IExternallyProvidedService implementation=IExternallyProvidedService]
2020-09-29 12:12:46.973 -05:00 [VRB] Resolved binding CollectionBinding[name= scope=Instance collectionType=HashSet`1 implementationCount=2]
2020-09-29 12:12:46.973 -05:00 [VRB] Resolved binding FactoryBinding[name= scope=Instance factoryType=IOtherServiceFactory genericType=IOtherService implementationType=OtherServiceImpl]
2020-09-29 12:12:46.974 -05:00 [VRB] Resolved binding DictionaryBinding[name= scope=Singleton dictionaryType=Dictionary`2 implementationCount=2]
```