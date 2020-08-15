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

    /// <inheritdoc cref="IInjector"/>
    internal class Injector : IInjector {

        internal ICache<object, Type, string> InstanceCache { get; }

        internal ICache<IBinding, Type, string> BindingCache { get; }

        internal ICache<Provider, Type, string> ProviderCache { get; }

        private List<IBindingHandler> BindingHandlers { get; }

        private ICreator Creator { get; }

        private IMethodInvoker MethodInvoker { get; }

        private IMethodBindingFactory MethodBindingFactory { get; }

        /// <summary>
        /// Creates the injector, initializes binding handlers, and gets all module bindings.
        /// </summary>
        /// <param name="modules">The modules to get bindings from.</param>
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

        /// <summary>
        /// Gets an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to get an instance of.</param>
        /// <returns>An instance of <paramref name="type"/>.</returns>
        internal object Get(Type type) {
            return Get(BindingCache.Get(type), type, null);
        }

        /// <summary>
        /// Gets an instance of <paramref name="type"/> with name <paramref name="name"/>.
        /// </summary>
        /// <param name="type">The type to get an instance of.</param>
        /// <param name="name">The name to get an instance of.</param>
        /// <returns>An instance of <paramref name="type"/> with <paramref name="name"/>.</returns>
        internal object Get(Type type, string name) {
            return Get(BindingCache.Get(type, name), type, name);
        }

        /// <summary>
        /// Gets an instance of <paramref name="binding"/> with <paramref name="type"/> and <paramref name="name"/>.
        /// </summary>
        /// <param name="binding">The binding to get an instance of.</param>
        /// <param name="type">The type of the binding.</param>
        /// <param name="name">The name of the binding.</param>
        /// <returns>The object instance.</returns>
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

        /// <summary>
        /// Gets a binding, either named or unnamed.
        /// </summary>
        /// <param name="type">The type to get bindings for.</param>
        /// <param name="name">The name of the binding to get. May be <c>null</c>.</param>
        /// <returns>The binding, or <c>null</c>.</returns>
        internal IBinding GetBinding(Type type, string name) {
            return name != null ? //
                BindingCache.Get(type, name) : //
                BindingCache.Get(type);
        }

        /// <summary>
        /// Checks if a binding is cached, either named or unnamed.
        /// </summary>
        /// <param name="type">The type to check if bindings exist for.</param>
        /// <param name="name">The name to check for bindings of.</param>
        /// <returns><c>true</c> if the type is cached, otherwise <c>false</c>.</returns>
        internal bool IsCached(Type type, string name) {
            return name != null ? //
                InstanceCache.IsCached(type, name) : //
                InstanceCache.IsCached(type);
        }

        /// <summary>
        /// Sets an instance for a binding.
        /// </summary>
        /// <param name="binding">The binding to set an instance for.</param>
        /// <param name="instance">The instance to set.</param>
        /// <param name="type">The type to set the instance for.</param>
        /// <param name="name">The name to set the instance for.</param>
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

        /// <summary>
        /// Gets an instance for a bindings type and name.
        /// </summary>
        /// <param name="type">The type to get an instance for.</param>
        /// <param name="name">The name to get an instance for.</param>
        /// <returns>An <c>object</c>, or <c>null</c>.</returns>
        internal object GetInstance(Type type, string name) {
            return name != null ? //
                InstanceCache.Get(type, name) : //
                InstanceCache.Get(type);
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
    }
}
