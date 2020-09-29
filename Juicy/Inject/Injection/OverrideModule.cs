using Juicy.Inject.Storage;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Juicy.Interfaces.Storage;
using Juicy.Reflection;
using System;
using System.Collections.Generic;

namespace Juicy.Inject.Injection {

    /// <summary>
    /// Internal module used to override bindings from another module.
    /// </summary>
    internal sealed class OverrideModule : AbstractModule {
        private static readonly IMethodBindingFactory MethodBindingFactory = new MethodBindingFactory(new Reflector());

        private AbstractModule BaseModule { get; }

        private AbstractModule OverriddenModule { get; }

        private ICache<IBinding, Type, string> BindingCache { get; }

        internal OverrideModule(AbstractModule module, AbstractModule toOverride) {
            BaseModule = module;
            OverriddenModule = toOverride;
            BindingCache = new InMemoryCache<IBinding, Type, string>();
        }

        internal override List<IBinding> GetBindings() {
            List<IBinding> bindings = new List<IBinding>();
            bindings.AddRange(BaseModule.GetBindings());

            // this will prevent 'silently' overriding bad bindings in the base module
            foreach (var binding in bindings) {
                CacheBinding(binding);
            }

            List<IBinding> overriddenBindings = new List<IBinding>();
            overriddenBindings.AddRange(OverriddenModule.GetBindings());

            foreach (var binding in overriddenBindings) {
                if (IsCached(binding.BaseType, binding.Name)) {
                    continue;
                }

                bindings.Add(binding);
                CacheBinding(binding);
            }

            return bindings;
        }

        private void CacheBinding(IBinding binding) {
            if (binding.Name != null) {
                if (!BindingCache.IsCached(binding.BaseType, binding.Name)) {
                    BindingCache.Cache(binding, binding.BaseType, binding.Name);
                } else {
                    throw new InvalidOperationException($"Duplicate binding for type {binding.BaseType} with name {binding.Name} detected.");
                }
            } else {
                if (!BindingCache.IsCached(binding.BaseType)) {
                    BindingCache.Cache(binding, binding.BaseType);
                } else {
                    throw new InvalidOperationException($"Duplicate binding for type {binding.BaseType} detected.");
                }
            }
        }

        private bool IsCached(Type type, string name) {
            return name != null ? //
                BindingCache.IsCached(type, name) : //
                BindingCache.IsCached(type);
        }
    }
}