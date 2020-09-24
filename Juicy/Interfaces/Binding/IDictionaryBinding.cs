using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Binding {

    /// <summary>
    /// Binds a single type (<see cref="IBinding.BaseType"/>) to multiple keyed implementations in a consumer-defined dictionary.
    /// </summary>
    public interface IDictionaryBinding : IBinding {

        /// <summary>
        /// All keyed types implementing <see cref="IBinding.BaseType"/>
        /// </summary>
        public Dictionary<object, Type> ImplementationTypes { get; }
    }
}
