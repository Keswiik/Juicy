using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection.Handler {
    internal class CollectionBindingHandler : IBindingHandler {

        private Injector Injector { get; }

        public CollectionBindingHandler(Injector injector) {
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
