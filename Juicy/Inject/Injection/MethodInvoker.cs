using Juicy.Interfaces.Injection;
using Juicy.Reflection;
using Juicy.Reflection.Interfaces;

namespace Juicy.Inject.Injection {

    /// <inheritdoc cref="IMethodInvoker"/>
    internal class MethodInvoker : IMethodInvoker {
        private Injector Injector { get; }

        private Reflector Reflector { get; }

        /// <summary>
        /// Creates a new method invoker with the specified parent injector and reflector.
        /// </summary>
        /// <param name="injector">The parent injector.</param>
        /// <param name="reflector">The reflector.</param>
        internal MethodInvoker(Injector injector, Reflector reflector) {
            Injector = injector;
            Reflector = reflector;
        }

        public object Invoke(object instance, ICachedMethod method) {
            // no need to check anything if the method has no parameters, just return fast
            if (method.Parameters.Count == 0) {
                return Reflector.Invoke(method, instance);
            }

            var parameters = new object[method.Parameters.Count];
            foreach (ICachedParameter parameter in method.Parameters) {
                parameters[parameter.Position] = Injector.Get(parameter.Type, parameter.Name);
            }

            return Reflector.Invoke(method, instance, parameters);
        }
    }
}