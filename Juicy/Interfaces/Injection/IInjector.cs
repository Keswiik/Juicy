using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Injection {

    /// <summary>
    /// The entrypoint for all injection.
    /// </summary>
    public interface IInjector {

        /// <summary>
        /// Get an unnamed instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to inject.</typeparam>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        T Get<T>();

        /// <summary>
        /// Get a named instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to inject.</typeparam>
        /// <param name="name">The name of the binding.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        T Get<T>(string name);
    }
}
