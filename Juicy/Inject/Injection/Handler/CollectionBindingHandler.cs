using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection.Handler {

    /// <summary>
    /// Handler used to create injections based on <see cref="CollectionBinding"/>.
    /// </summary>
    internal sealed class CollectionBindingHandler : AbstractBindingHander {

        /// <summary>
        /// Create a new binding handler with the specified parent <paramref name="injector"/>.
        /// </summary>
        /// <param name="injector">The parent injector to use.</param>
        internal CollectionBindingHandler(Injector injector, ILoggerFactory loggerFactory) : base(injector, loggerFactory) {
        }

        public override object Handle(IBinding binding, Type type, string name) {
            var collectionBinding = binding as CollectionBinding;
            bool hitCache = collectionBinding.Scope == Constants.BindingScope.Singleton;
            bool isCached = Injector.IsCached(collectionBinding.BaseType, collectionBinding.Name);

            if (!hitCache || !isCached) {
                logger?.LogTrace("Creating implementations for {Binding}.", collectionBinding);
                // TODO: determine if I am going to be lynched for doing this. I don't know how bad dynamic is for performance.
                dynamic collection = Activator.CreateInstance(collectionBinding.BaseType);
                foreach (Type implementationType in collectionBinding.ImplementationTypes) {
                    dynamic value = Injector.Get(implementationType, null);
                    collection.Add(value);
                }

                if (hitCache) {
                    logger?.LogTrace("Caching instance for {Binding}.", collectionBinding);
                    Injector.SetInstance(collectionBinding, collection, collectionBinding.BaseType, collectionBinding.Name);
                }

                return collection;
            }

            return Injector.GetInstance(collectionBinding.BaseType, collectionBinding.Name);
        }
    }
}
