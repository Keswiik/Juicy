using Juicy.Constants;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Binding {

    /// <summary>
    /// Common properties shared across <b>all</b> bindings.
    /// </summary>
    public interface IBinding {

        /// <summary>
        /// The type the binding was created against.
        /// </summary>
        /// <remarks>
        /// Used to retrieve bindings from cache.
        /// </remarks>
        public Type BaseType { get; }

        /// <summary>
        /// The scope of the binding.
        /// </summary>
        /// <remarks>
        /// Controls the situations in which new instances of a binding are created.
        /// </remarks>
        public BindingScope Scope { get; }

        /// <summary>
        /// The type of the binding.
        /// </summary>
        public BindingType Type { get; }

        /// <summary>
        /// The name of the binding. Can be <c>null</c>.
        /// </summary>
        /// <remarks>
        /// Any binding with a <c>null</c> is considered an unnamed binding.
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// The module the binding originated from.
        /// </summary>
        public IModule Module { get; }
    }
}
