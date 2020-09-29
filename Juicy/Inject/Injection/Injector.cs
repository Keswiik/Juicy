using Juicy.Constants;
using Juicy.Inject.Binding;
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

        internal Injector ParentInjector { get; }

        private Dictionary<BindingType, IBindingHandler> BindingHandlers { get; }

        private ICreator Creator { get; }

        private IMethodInvoker MethodInvoker { get; }

        private IMethodBindingFactory MethodBindingFactory { get; }


        /// <summary>
        /// Creates the injector, initializes binding handlers, and gets all module bindings.
        /// </summary>
        /// <param name="modules">The modules to get bindings from.</param>
        internal Injector(params AbstractModule[] modules) : this(null, modules) { }

        /// <summary>
        /// Creates an injector with a parent to delegate requests to if there is no binding.
        /// </summary>
        /// <param name="parentInjector">The parent to this injector.</param>
        /// <param name="modules">The modules to get bindings from.</param>
        internal Injector(Injector parentInjector, params AbstractModule[] modules) {
            ParentInjector = parentInjector;
            BindingCache = new InMemoryCache<IBinding, Type, string>();
            InstanceCache = new InMemoryCache<object, Type, string>();
            ProviderCache = new InMemoryCache<Provider, Type, string>();

            var reflector = new Reflector();
            Creator = new Creator(this, reflector);
            MethodInvoker = new MethodInvoker(this, reflector);
            MethodBindingFactory = new MethodBindingFactory(reflector);

            BindingHandlers = new Dictionary<BindingType, IBindingHandler> {
                { BindingType.Collection, new CollectionBindingHandler(this) },
                { BindingType.Concrete, new ConcreteBindingHandler(this, Creator) },
                { BindingType.Factory, new FactoryBindingHandler(this, Creator) },
                { BindingType.Dictionary, new DictionaryBindingHandler(this, Creator) },
                { BindingType.Method, new MethodBindingHandler(this, MethodInvoker) },
                { BindingType.None, new NoBindingHandler(this, Creator) },
                { BindingType.Provider, new ProviderBindingHandler(this) }
            };

            Queue<AbstractModule> moduleInstallationQueue = new Queue<AbstractModule>(modules);

            while (moduleInstallationQueue.Count > 0) {
                var module = moduleInstallationQueue.Dequeue();
                module.Configure();

                var bindings = new List<IBinding>();
                if (module is OverrideModule) {
                    bindings.AddRange(module.GetBindings());
                } else {
                    bindings.AddRange(MethodBindingFactory.CreateBindings(module));
                    bindings.AddRange(module.GetBindings());
                }

                foreach (IBinding binding in bindings) {
                    CacheBinding(binding);
                    BindingHandlers[binding.Type].Initialize(binding);
                }

                foreach (var childModule in module.InstalledModules) {
                    moduleInstallationQueue.Enqueue(childModule);
                }
            }

            // create a binding for the injector itself after everything has gone through
            // binding is weird as it has no module, but this should never cause an error
            var injectorBinding = (new ConcreteBinding.ConcreteBindingBuilder(typeof(IInjector), BindingType.Concrete, null)
                .ToInstance(this)
                .In(BindingScope.Singleton) as IBuilder)
                .Build();
            CacheBinding(injectorBinding);
            BindingHandlers[injectorBinding.Type].Initialize(injectorBinding);
        }

        public T Get<T>() {
            return (T)Get(typeof(T), null);
        }

        public T Get<T>(string name) {
            return (T)Get(typeof(T), name);
        }

        public IInjector CreateChildInjector(params AbstractModule[] modules) {
            return new Injector(this, modules);
        }

        /// <summary>
        /// Gets an instance of <paramref name="type"/> with name <paramref name="name"/>.
        /// </summary>
        /// <param name="type">The type to get an instance of.</param>
        /// <param name="name">The name to get an instance of.</param>
        /// <returns>An instance of <paramref name="type"/> with <paramref name="name"/>.</returns>
        internal object Get(Type type, string name) {
            return Get( //
                name == null ?
                    BindingCache.Get(type) : //
                    BindingCache.Get(type, name), 
                type, name);
        }

        /// <summary>
        /// Gets an instance of <paramref name="binding"/> with <paramref name="type"/> and <paramref name="name"/>.
        /// </summary>
        /// <param name="binding">The binding to get an instance of.</param>
        /// <param name="type">The type of the binding.</param>
        /// <param name="name">The name of the binding.</param>
        /// <returns>The object instance.</returns>
        internal object Get(IBinding binding, Type type, string name) {
            return BindingHandlers[binding?.Type ?? BindingType.None].Handle(binding, type, name);
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
