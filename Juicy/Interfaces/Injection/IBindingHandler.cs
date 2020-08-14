using Juicy.Interfaces.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Injection {
    internal interface IBindingHandler {
        internal bool CanHandle(IBinding binding);
        internal bool NeedsInitialized(IBinding binding);
        internal object Handle(IBinding binding, Type type, string name);
        internal void Initialize(IBinding binding);
    }
}
