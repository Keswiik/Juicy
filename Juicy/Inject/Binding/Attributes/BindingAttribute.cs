using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Binding.Attributes {
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BindingAttribute : Attribute {

    }
}
