using Juicy.Constants;
using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;

namespace Juicy.Inject.Injection.Handler {

    /// <summary>
    /// Handler used to create injections based on <see cref="DictionaryBinding"/>.
    /// </summary>
    internal sealed class DictionaryBindingHandler : AbstractBindingHander {

        /// <summary>
        /// Creates a new dictionary binding handler.
        /// </summary>
        /// <param name="injector">The injector to forward implementing type requests to.</param>
        /// <param name="loggerFactory">The logger factory used to create an ILogger.</param>
        internal DictionaryBindingHandler(Injector injector, ILoggerFactory loggerFactory) : base(injector, loggerFactory) {
        }

        /*
         * How this method works:
         *      See the instance shouldn't be cached, or if it isn't cached
         *      If so, create an instance of our new dictionary
         *      Loop over all keys, request instance of implementing types from the injector
         *      Cache dictionary if necessary
         */

        public override object Handle(IBinding binding, Type type, string name) {
            var dictionaryBinding = binding as DictionaryBinding;
            var hitCache = dictionaryBinding.Scope == BindingScope.Singleton;
            var isCached = Injector.IsCached(dictionaryBinding.BaseType, dictionaryBinding.Name);

            if (!hitCache || !isCached) {
                logger?.LogTrace("Creating implementation for {Binding}.", dictionaryBinding);
                IDictionary dictionary = (IDictionary)Activator.CreateInstance(dictionaryBinding.BaseType);
                foreach (var key in dictionaryBinding.ImplementationTypes.Keys) {
                    object instance = Injector.Get(dictionaryBinding.ImplementationTypes[key], null);
                    dictionary.Add(key, instance);
                }

                if (hitCache) {
                    logger?.LogTrace("Caching implementation for {Binding}.", dictionaryBinding);
                    Injector.SetInstance(dictionaryBinding, dictionary, dictionaryBinding.BaseType, dictionaryBinding.Name);
                }

                return dictionary;
            }

            return Injector.GetInstance(dictionaryBinding.BaseType, dictionaryBinding.Name);
        }
    }
}