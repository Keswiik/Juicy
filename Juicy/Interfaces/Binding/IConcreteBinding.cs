using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Binding {
    
    /// <summary>
    /// Binds a single type (<see cref="IBinding.BaseType"/>) to another type or instance.
    /// </summary>
    public interface IConcreteBinding : IBinding {
        
        /// <summary>
        /// The type implementing <see cref="IBinding.BaseType"/>.
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        /// An instance of an implementation of <see cref="IBinding.BaseType"/>.
        /// </summary>
        /// <remarks>
        /// Mainly used to form named bindings of primitive types.
        /// </remarks>
        public object Instance { get; }

        /// <summary>
        /// The provider used to produce an instance of <see cref="IBinding.BaseType"/>
        /// </summary>
        public Type Provider { get; }
    }
}
