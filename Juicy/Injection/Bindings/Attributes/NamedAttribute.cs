using System;

namespace Juicy.Injection.Attributes {

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class NamedAttribute : Attribute {
        public NamedAttribute(string name) {
            Name = name;
        }

        public string Name { get; }
    }
}