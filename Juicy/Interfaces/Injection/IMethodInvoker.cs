using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Injection {

    /// <summary>
    /// Invokes methods using reflection.
    /// </summary>
    internal interface IMethodInvoker {

        /// <summary>
        /// Invoke the target <paramref name="method"/> on <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The target object to call <paramref name="method"/> on.</param>
        /// <param name="method">The method to invoke.</param>
        /// <returns>An object if one was returned by <paramref name="method"/>, otherwise <c>null</c>.</returns>
        internal object Invoke(object instance, ICachedMethod method);
    }
}
