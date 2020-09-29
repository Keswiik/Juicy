using System;

namespace Juicy.Inject.Binding.Attributes {

    /// <summary>
    /// Allows the setting of named bindings on methods and method parameters.
    /// </summary>
    /// <remarks>
    /// When used on a module method in conjuction with <see cref="ProvidesAttribute"/>, it will create a named injection bound to the method. When used on method parameters, it allows the injector to look for specific instances of a value.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class NamedAttribute : Attribute {

        public NamedAttribute(string name) {
            Name = name;
        }

        public string Name { get; }
    }
}