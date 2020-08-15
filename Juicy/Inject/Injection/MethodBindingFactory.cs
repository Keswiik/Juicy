using Juicy.Constants;
using Juicy.Inject.Binding;
using Juicy.Inject.Binding.Attributes;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Juicy.Reflection;
using Juicy.Reflection.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection {

    /// <inheritdoc cref="IMethodBindingFactory"/>
    internal class MethodBindingFactory : IMethodBindingFactory {

        private static readonly Type ProvidesAttributeType = typeof(ProvidesAttribute);

        private Injector Injector { get; }

        private Reflector Reflector { get; }

        internal MethodBindingFactory(Injector injector, Reflector reflector) {
            Injector = injector;
            Reflector = reflector;
        }

        List<IBinding> IMethodBindingFactory.CreateBindings(IModule module) {
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
                } else if (name != null && Injector.BindingCache.IsCached(baseType, name)) {
                    throw new InvalidOperationException($"Named binding from provider method named {method.Name} in {module.GetType().FullName} conflicts with an existing binding of name {name}.");
                } else if (name == null && Injector.BindingCache.IsCached(baseType)) {
                    throw new InvalidOperationException($"Binding from provided method named {method.Name} in {module.GetType().FullName} conflicts with an existing binding.");
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
