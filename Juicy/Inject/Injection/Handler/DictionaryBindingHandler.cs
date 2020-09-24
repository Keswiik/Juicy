using Juicy.Constants;
using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Juicy.Inject.Injection.Handler {

    /// <summary>
    /// Handler used to create injections based on <see cref="DictionaryBinding"/>.
    /// </summary>
    sealed internal class DictionaryBindingHandler : IBindingHandler {

        private Injector Injector { get; }

        private ICreator Creator { get; }

        /// <summary>
        /// Creates a new dictionary binding handler.
        /// </summary>
        /// <param name="injector">The injector to forward implementing type requests to.</param>
        /// <param name="creator">The creator to use.</param>
        internal DictionaryBindingHandler(Injector injector, ICreator creator) {
            Injector = injector;
            Creator = creator;
        }

        /*
         * How this method works:
         *      See the instance shouldn't be cached, or if it isn't cached
         *      If so, create an instance of our new dictionary
         *      Loop over all keys, request instance of implementing types from the injector
         *      Cache dictionary if necessary
         */
        object IBindingHandler.Handle(IBinding binding, Type type, string name) {
            var mapBinding = binding as DictionaryBinding;
            var hitCache = mapBinding.Scope == BindingScope.Singleton;
            var isCached = Injector.IsCached(mapBinding.BaseType, mapBinding.Name);

            if (!hitCache || !isCached) {
                IDictionary dictionary = (IDictionary)Activator.CreateInstance(mapBinding.BaseType);
                foreach (var key in mapBinding.ImplementationTypes.Keys) {
                    dynamic instance = Injector.Get(mapBinding.ImplementationTypes[key], null);
                    dictionary.Add(key, instance);
                }

                if (hitCache) {
                    Injector.SetInstance(mapBinding, dictionary, mapBinding.BaseType, mapBinding.Name);
                }

                return dictionary;
            }

            return Injector.GetInstance(mapBinding.BaseType, mapBinding.Name);
        }

        void IBindingHandler.Initialize(IBinding binding) {
        }
    }
}
