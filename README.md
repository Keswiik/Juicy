# Juicy
[Documentation](https://keswiik.github.io/Juicy/)

Juicy is a lightweight recreation of Google's [Guice](https://github.com/google/guice) dependency injection library in C#.

This project is not yet complete and is missing several features from Guice.
- [Untargeted bindings](https://github.com/google/guice/wiki/UntargettedBindings)
- [Explicitly implementing and binding an `IProvider<T>`](https://github.com/google/guice/wiki/ProviderBindings)
- [Binding to arbitrary attributes](https://github.com/google/guice/wiki/BindingAnnotations)
- [Binding to maps](https://github.com/google/guice/wiki/Multibindings)
- Overriding bindings of another module
- Installing dependent modules
- Injector hierarchy / child injectors
- Allow injector to inject itself