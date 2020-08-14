using Juicy.Inject.Binding.Attributes;
using Juicy.Interfaces.Injection;
using Juicy.Reflection;
using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection {
    internal class MethodInvoker : IMethodInvoker {

        private Injector Injector { get; }

        private Reflector Reflector { get; }

        internal MethodInvoker(Injector injector, Reflector reflector) {
            Injector = injector;
            Reflector = reflector;
        }

        object IMethodInvoker.Invoke(object instance, ICachedMethod method) {
            if (method.Parameters.Count == 0) {
                return Reflector.Invoke(method, instance);
            }

            var parameters = new object[method.Parameters.Count];
            foreach (ICachedParameter parameter in method.Parameters) {
                var name = parameter.GetAttribute<NamedAttribute>()?.Name;
                parameters[parameter.Position] = name != null ? //
                    Injector.Get(parameter.Type, name) : //
                    Injector.Get(parameter.Type);
            }

            return Reflector.Invoke(method, instance, parameters);
        }
    }
}
