using System;

namespace Juicy.Inject.Binding.Attributes {

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class NamedAttribute : Attribute {
        public NamedAttribute(string name) {
            Name = name;
        }

        public string Name { get; }
    }
}