using Juicy.Interfaces.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Injection {
    
    /// <summary>
    /// Handles requests for specific bindings.
    /// </summary>
    /// <remarks>
    /// This is used by the <see cref="IInjector"/> to modularize injection logic.
    /// </remarks>
    internal interface IBindingHandler {

        /// <summary>
        /// Checks if the current handler can accept the provided <paramref name="binding"/>.
        /// </summary>
        /// <param name="binding">A binding requested by an <see cref="IInjector"/>.</param>
        /// <returns><c>true</c> if the handler can accept the binding, otherwise <c>false</c>.</returns>
        internal bool CanHandle(IBinding binding);

        /// <summary>
        /// Checks if the binding requires initializing before it can be used.
        /// </summary>
        /// <remarks>This should be called <b>after</b> checking if the handler can accept the <paramref name="binding"/>.</remarks>
        /// <param name="binding">The binding to check.</param>
        /// <returns><c>true</c> if the binding requires initialization, otherwise <c>false</c>.</returns>
        internal bool NeedsInitialized(IBinding binding);

        /// <summary>
        /// Process the request for <paramref name="binding"/> and return the corresponding value.
        /// </summary>
        /// <remarks>
        /// Sometimes bindings are <c>null</c>. Null bindings are valid, so we provide the base request information (<paramref name="type"/> and <paramref name="name"/>) as a contingency.
        /// </remarks>
        /// <param name="binding">The binding to create an object from.</param>
        /// <param name="type">The raw type that was requested.</param>
        /// <param name="name">The raw name that was requested.</param>
        /// <returns>The requested object.</returns>
        internal object Handle(IBinding binding, Type type, string name);

        /// <summary>
        /// Initializes a binding.
        /// </summary>
        /// <remarks>Should only be called after checking if the binding requires initialization.</remarks>
        /// <param name="binding">The binding to initialize.</param>
        internal void Initialize(IBinding binding);
    }
}
