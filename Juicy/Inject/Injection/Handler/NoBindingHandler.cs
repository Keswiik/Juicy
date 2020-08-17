using Juicy.Constants;
using Juicy.Inject.Injection.Provide;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Juicy.Inject.Injection.Handler {

    /// <summary>
    /// Handler used to create injections when no binding is available.
    /// </summary>
    /// <remarks>
    /// Usage of this handler usually involves injecting either <see cref="IProvider{T}"/> or a concrete class with no explicit binding specified.
    /// </remarks>
    internal class NoBindingHandler : IBindingHandler {

        private readonly static Type ProviderType = typeof(IProvider<>);

        private readonly static Type ParameterizedProviderType = typeof(ParameterizedProvider<>);

        private const BindingFlags ProviderBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        private Injector Injector { get; }

        private ICreator Creator { get; }

        /// <summary>
        /// Creates a handler with the specified parent injector and creator.
        /// </summary>
        /// <param name="injector">The parent injector to use.</param>
        /// <param name="creator">The creator to use.</param>
        internal NoBindingHandler(Injector injector, ICreator creator) {
            Injector = injector;
            Creator = creator;
        }

        /*
         * How this method works:
         *      If a provider was requested
         *          create the provider if we don't have it cached
         *          return the provider
         *      If it isn't a provider, and we have a parent injector, pass the request up
         *      If we have no parent injector and the request is for a a non-value type that is can be created, try to create it
         */
        object IBindingHandler.Handle(IBinding binding, Type type, string name) {
            if (IsProvider(type)) {

                // providers should ALWAYS be cached after they are created. No reason to create multiple for the same type.
                // they will delegate to the same binding anyways.
                if (!Injector.ProviderCache.IsCached(type)) {
                    var underlyingType = type.GetGenericArguments()[0];
                    binding = Injector.GetBinding(underlyingType, name);
                    var cacheInstance = binding?.Scope == BindingScope.Singleton;
                    var provider = (Provider)Activator.CreateInstance(ParameterizedProviderType //
                        .MakeGenericType(underlyingType), ProviderBindingFlags, null, new object[] { cacheInstance, this }, null);
                    if (name != null) {
                        Injector.ProviderCache.Cache(provider, type, name);
                    } else {
                        Injector.ProviderCache.Cache(provider, type);
                    }
                }

                return (name != null) ? //
                    Injector.ProviderCache.Get(type, name) : //
                    Injector.ProviderCache.Get(type);
            } else if (Injector.ParentInjector != null) {
                return Injector.ParentInjector.Get(type, name);
            } else if (!type.IsInterface && !type.IsAbstract && !type.IsValueType) {
                // if it isn't a provider, and is a concrete class...we'll give it a try.
                return Creator.CreateInstance(type);
            } else {
                throw new InvalidOperationException($"Cannot create an instance of type {type.FullName} that has no binding");
            }
        }

        void IBindingHandler.Initialize(IBinding binding) { }

        private bool IsProvider(Type type) {
            return type.IsGenericType && type.GetGenericTypeDefinition() == ProviderType;
        }
    }
}
