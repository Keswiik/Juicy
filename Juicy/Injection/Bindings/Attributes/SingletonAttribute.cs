using Juicy.Injection.Bindings;
using Juicy.Injection.Interfaces;
using System;

namespace Juicy.Injection.Attributes {

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SingletonAttribute : Attribute, IScopeProvider {
        public Scope Scope => Scope.Singleton;
    }
}