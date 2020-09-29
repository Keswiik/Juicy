using Juicy.Interfaces.Binding;
using System;

namespace Juicy.Interfaces.Injection {

    /// <summary>
    /// Handles requests for specific bindings.
    /// </summary>
    /// <remarks>
    /// This is used by the <see cref="IInjector"/> to modularize injection logic.
    /// </remarks>
    internal interface IBindingHandler {

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
        object Handle(IBinding binding, Type type, string name);

        /// <summary>
        /// Initializes a binding.
        /// </summary>
        /// <remarks>Should only be called after checking if the binding requires initialization.</remarks>
        /// <param name="binding">The binding to initialize.</param>
        void Initialize(IBinding binding);
    }
}