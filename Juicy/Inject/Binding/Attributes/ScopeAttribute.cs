using Juicy.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Binding.Attributes {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ScopeAttribute : Attribute {

        public ScopeAttribute(BindingScope scope) {
            Scope = scope;
        }

        public BindingScope Scope { get; }
    }
}
