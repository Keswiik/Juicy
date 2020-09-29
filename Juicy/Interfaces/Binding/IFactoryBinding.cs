using System;

namespace Juicy.Interfaces.Binding {

    /// <summary>
    /// Binds a interface to a proxy that can be used to produce multiple instances of <see cref="IFactoryBinding.ImplementationType"/>.
    /// </summary>
    public interface IFactoryBinding : IBinding {

        /// <summary>
        /// The generic interface or class that is being implemented by <see cref="IFactoryBinding.ImplementationType"/>.
        /// </summary>
        public Type GenericType { get; }

        /// <summary>
        /// The type that will be instantiated whenever factory methods are called.
        /// </summary>
        public Type ImplementationType { get; }
    }
}