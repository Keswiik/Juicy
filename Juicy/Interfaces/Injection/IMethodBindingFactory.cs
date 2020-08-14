using Juicy.Interfaces.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Injection {
    internal interface IMethodBindingFactory {
        internal List<IBinding> CreateBindings(IModule module);
    }
}
