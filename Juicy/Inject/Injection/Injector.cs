using Juicy.Inject.Injection.Handler;
using Juicy.Inject.Injection.Provide;
using Juicy.Inject.Storage;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Juicy.Interfaces.Storage;
using Juicy.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection {
    internal class Injector : IInjector {

        internal ICache<object, Type, string> InstanceCache { get; }
        internal ICache<IBinding, Type, string> BindingCache { get; }
        internal ICache<Provider, Type, string> ProviderCache { get; }

        private List<IBindingHandler> BindingHandlers { get; }
        private ICreator Creator { get; }
        private IMethodInvoker MethodInvoker { get; }
        private IMethodBindingFactory MethodBindingFactory { get; }

        internal Injector(params AbstractModule[] modules) {
            BindingCache = new Cache<IBinding, Type, string>();
            InstanceCache = new Cache<object, Type, string>();
            ProviderCache = new Cache<Provider, Type, string>();

            var reflector = new Reflector();
            Creator = new Creator(this, reflector);
            MethodInvoker = new MethodInvoker(this, reflector);
            MethodBindingFactory = new MethodBindingFactory(this, reflector);

            BindingHandlers = new List<IBindingHandler> {
                new FactoryBindingHandler(this, Creator),
                new ConcreteBindingHandler(this, Creator),
                new CollectionBindingHandler(this),
                new NoBindingHandler(this, Creator),
                new MethodBindingHandler(this, MethodInvoker)
            };

            foreach (AbstractModule module in modules) {
                var bindings = new List<IBinding>();
                bindings.AddRange(MethodBindingFactory.CreateBindings(module));

                module.Configure();
                bindings.AddRange(module.GetBindings());

                foreach (IBinding binding in bindings) {
                    CacheBinding(binding);
                    foreach (IBindingHandler handler in BindingHandlers) {
                        if (handler.CanHandle(binding) && handler.NeedsInitialized(binding)) {
                            handler.Initialize(binding);
                        }
                    }
                }
            }
        }

        public T Get<T>() {
            return (T)Get(typeof(T));
        }

        public T Get<T>(string name) {
            return (T)Get(typeof(T), name);
        }

        internal object Get(Type type) {
            return Get(BindingCache.Get(type), type, null);
        }

        internal object Get(Type type, string name) {
            return Get(BindingCache.Get(type, name), type, name);
        }

        internal object Get(IBinding binding, Type type, string name) {
            object instance = null;
            bool isHandled = false;
            foreach (IBindingHandler handler in BindingHandlers) {
                isHandled = handler.CanHandle(binding);
                if (isHandled) {
                    instance = handler.Handle(binding, type, name);
                    break;
                }
            }

            if (!isHandled) {
                throw new InvalidOperationException($"Failed to handle binding for type {type.FullName}.");
            }

            return instance;
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

        internal IBinding GetBinding(Type type, string name) {
            return name != null ? //
                BindingCache.Get(type, name) : //
                BindingCache.Get(type);
        }

        internal bool IsCached(Type type, string name) {
            return name != null ? //
                InstanceCache.IsCached(type, name) : //
                InstanceCache.IsCached(type);
        }

        internal void SetInstance(IBinding binding, object instance, Type type, string name) {
            if (instance == null) {
                throw new InvalidOperationException($"Attempted to bind null to instance for binding of type {binding.BaseType}");
            }else if (name != null) {
                if (!InstanceCache.IsCached(type, name)) {
                    InstanceCache.Cache(instance, type, name);
                } else {
                    throw new InvalidOperationException($"Binding for type {binding.BaseType} with name {binding.Name} attempts to override cached instance for {type}.");
                }
            } else {
                if (!InstanceCache.IsCached(type)) {
                    InstanceCache.Cache(instance, type);
                } else {
                    throw new InvalidOperationException($"Binding for type {binding.BaseType} attempts to override cached instance for {type}.");
                }
            }
        }

        internal object GetInstance(Type type, string name) {
            return name != null ? //
                InstanceCache.Get(type, name) : //
                InstanceCache.Get(type);
        }
    }
}
