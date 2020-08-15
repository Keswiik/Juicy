using System;

namespace Juicy.Inject.Binding.Attributes {

    /// <summary>
    /// Tells the injector which constructors can be used to inject values into.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class InjectAttribute : Attribute {
    }
}