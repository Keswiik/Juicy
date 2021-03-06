﻿using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Microsoft.Extensions.Logging;
using System;

namespace Juicy.Inject.Injection.Handler {

    /// <summary>
    /// Handler used to create injections when a concrete type is explicitly bound to an implementation of <c>IProvider</c>.
    /// </summary>
    internal sealed class ProviderBindingHandler : AbstractBindingHander {

        /// <summary>
        /// Creates a handler with the specified parent injector.
        /// </summary>
        /// <param name="injector">The parent injector to use.</param>
        /// <param name="loggerFactory">The logger factory used to create an ILogger.</param>
        internal ProviderBindingHandler(Injector injector, ILoggerFactory loggerFactory) : base(injector, loggerFactory) { }

        /*
         * How this method works:
         *      If the value is cached, return the cached value
         *      If the value is not cached or is not a singleton
         *          Get the provider
         *          Get value from provider
         *          Cache the new value and return it
         */

        public override object Handle(IBinding binding, Type type, string name) {
            var concreteBinding = binding as ConcreteBinding;
            var hitCache = concreteBinding.Scope == Constants.BindingScope.Singleton;
            var isCached = Injector.IsCached(concreteBinding.ImplementationType, concreteBinding.Name);

            if (!hitCache || !isCached) {
                logger?.LogTrace("Creating implementation using a provider for {Binding}.", concreteBinding);
                dynamic provider = GetProvider(concreteBinding.Provider, concreteBinding.Name);
                dynamic instance = provider.Get();
                if (hitCache) {
                    logger?.LogTrace("Caching implementation for {Binding}.", concreteBinding);
                    Injector.SetInstance(concreteBinding, instance, concreteBinding.ImplementationType, concreteBinding.Name);
                }

                return instance;
            }

            return Injector.GetInstance(concreteBinding.ImplementationType, concreteBinding.Name);
        }

        /// <summary>
        /// Gets the provider from <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of the provider being requested.</param>
        /// <param name="name">The name of the binding the provider is for.</param>
        /// <returns>An instance of the provider.</returns>
        private dynamic GetProvider(Type type, string name) {
            var isNamed = name != null;
            var isCached = isNamed ?
                Injector.ProviderCache.IsCached(type, name) :
                Injector.ProviderCache.IsCached(type);

            if (isCached) {
                return isNamed ?
                        Injector.ProviderCache.Get(type, name) :
                        Injector.ProviderCache.Get(type);
            } else {
                return Injector.Get(type, name);
            }
        }
    }
}