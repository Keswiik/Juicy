using Juicy.Constants;
using Juicy.Inject.Binding;
using Juicy.Inject.Binding.Attributes;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Juicy.Reflection;
using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;

namespace Juicy.Inject.Injection {

    /// <inheritdoc cref="IMethodBindingFactory"/>
    internal class MethodBindingFactory : IMethodBindingFactory {
        private static readonly Type ProvidesAttributeType = typeof(ProvidesAttribute);

        private Reflector Reflector { get; }

        internal MethodBindingFactory(Reflector reflector) {
            Reflector = reflector;
        }

        public List<IBinding> CreateBindings(IModule module) {
            List<ICachedMethod> providedMethods = Reflector.GetAttributedMethods(module.GetType(), ProvidesAttributeType);
            if (providedMethods.Count == 0) {
                return new List<IBinding>();
            }

            List<IBinding> bindings = new List<IBinding>();
            foreach (var method in providedMethods) {
                var baseType = method.ReturnType;
                var scope = method.Scope;
                var name = method.BindingName;

                if (name != null && string.IsNullOrWhiteSpace(name)) {
                    throw new InvalidOperationException($"Invalid empty or whitespace name on provider method named {method.Name} in {module.GetType().FullName}.");
                }

                IBuilder builder = new MethodBinding.MethodBindingBuilder(baseType, BindingType.Method, module) //
                    .In(scope) //
                    .Named(name) //
                    .Method(method);

                bindings.Add(builder.Build());
            }

            return bindings;
        }
    }
}