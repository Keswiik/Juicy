using System;

namespace Juicy.Inject.Binding.Attributes {

    /// <summary>
    /// Marks a module method to used for injection bindings.
    /// </summary>
    /// <remarks>
    /// Don't both using this on any methods outside of a module - they won't be detected.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ProvidesAttribute : Attribute {
    }
}