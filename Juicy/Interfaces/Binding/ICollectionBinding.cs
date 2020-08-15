using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Binding {

    /// <summary>
    /// Binds a single type (<see cref="IBinding.BaseType"/>) to multiple implementations in a consumer-defined collection.
    /// </summary>
    public interface ICollectionBinding : IBinding {

        /// <summary>
        /// All types implementing <see cref="IBinding.BaseType"/>.
        /// </summary>
        public List<Type> ImplementationTypes { get; }
    }
}
