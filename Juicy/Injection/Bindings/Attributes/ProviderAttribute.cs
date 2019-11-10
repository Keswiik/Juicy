using System;

namespace Juicy.Injection.Attributes {

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ProviderAttribute : Attribute {
    }
}