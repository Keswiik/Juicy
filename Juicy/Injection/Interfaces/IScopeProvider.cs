using Juicy.Injection.Bindings;

namespace Juicy.Injection.Interfaces {

    internal interface IScopeProvider {
        Scope Scope { get; }
    }
}