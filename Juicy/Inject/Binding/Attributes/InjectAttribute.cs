using System;

namespace Juicy.Inject.Binding.Attributes {

    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class InjectAttribute : Attribute {
    }
}