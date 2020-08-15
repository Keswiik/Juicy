using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection.Handler {

    /// <summary>
    /// Handler used to create injections based on <see cref="CollectionBinding"/>.
    /// </summary>
    internal class CollectionBindingHandler : IBindingHandler {

        private Injector Injector { get; }

        /// <summary>
        /// Create a new binding handler with the specified parent <paramref name="injector"/>.
        /// </summary>
        /// <param name="injector">The parent injector to use.</param>
        internal CollectionBindingHandler(Injector injector) {
            Injector = injector;
        }

        bool IBindingHandler.CanHandle(IBinding binding) {
            return binding is CollectionBinding;
        }

        object IBindingHandler.Handle(IBinding binding, Type type, string name) {
            var collectionBinding = binding as CollectionBinding;
            bool hitCache = collectionBinding.Scope == Constants.BindingScope.Singleton;
            bool isCached = Injector.IsCached(collectionBinding.BaseType, collectionBinding.Name);

            if (!hitCache || !isCached) {
                // TODO: determine if I am going to be lynched for doing this. I don't know how bad dynamic is for performance.
                dynamic collection = Activator.CreateInstance(collectionBinding.BaseType);
                foreach (Type implementationType in collectionBinding.ImplementationTypes) {
                    dynamic value = Injector.Get(implementationType);
                    collection.Add(value);
                }

                if (hitCache) {
                    Injector.SetInstance(collectionBinding, collection, collectionBinding.BaseType, collectionBinding.Name);
                }

                return collection;
            }

            return Injector.GetInstance(collectionBinding.BaseType, collectionBinding.Name);
        }

        void IBindingHandler.Initialize(IBinding binding) {
        }

        bool IBindingHandler.NeedsInitialized(IBinding binding) {
            return false;
        }
    }
}
