using Juicy.Reflection.Interfaces;

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
        object Invoke(object instance, ICachedMethod method);
    }
}