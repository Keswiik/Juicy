using System;

namespace Juicy.Inject.Binding.Attributes {

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BindingAttribute : Attribute {
    }
}