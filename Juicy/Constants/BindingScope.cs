using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Constants {

    /// <summary>
    /// Determines how an object should be instantiated.
    /// </summary>
    public enum BindingScope {

        /// <summary>
        /// Never use the cache. Always create a new instance when injecting.
        /// </summary>
        Instance,

        /// <summary>
        /// Use the cache. Create a new instance <b>only</b> if the cache does not contain one already.
        /// </summary>
        Singleton
    }
}
