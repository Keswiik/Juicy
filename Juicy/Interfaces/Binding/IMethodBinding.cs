using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Binding {

    /// <summary>
    /// Binds a method to its return type.
    /// </summary>
    public interface IMethodBinding : IBinding {

        /// <summary>
        /// The method to invoke when the binding is called.
        /// </summary>
        public ICachedMethod Method { get; }
    }
}
