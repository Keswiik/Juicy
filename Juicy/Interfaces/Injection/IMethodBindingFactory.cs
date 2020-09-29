using Juicy.Interfaces.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Injection {

    /// <summary>
    /// Produces bindings for methods within a module.
    /// </summary>
    internal interface IMethodBindingFactory {

        /// <summary>
        /// Scans for valid methods in <paramref name="module"/> that can produce an <see cref="IBinding"/>.
        /// </summary>
        /// <param name="module">The module to search for methods in.</param>
        /// <returns>A list of bindings which may be empty.</returns>
        List<IBinding> CreateBindings(IModule module);
    }
}
