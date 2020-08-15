using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection.Handler {

    /// <summary>
    /// Handler used to create injections based on <see cref="ConcreteBinding"/>
    /// </summary>
    internal class ConcreteBindingHandler : IBindingHandler {

        private Injector Injector { get; }

        private ICreator Creator { get; }

        /// <summary>
        /// Creates a new handler with the specified parent injector and instance creator.
        /// </summary>
        /// <param name="injector">The parent injector to use.</param>
        /// <param name="creator">The creator to use.</param>
        internal ConcreteBindingHandler(Injector injector, ICreator creator) {
            Injector = injector;
            Creator = creator;
        }

        object IBindingHandler.Handle(IBinding binding, Type type, string name) {
            var concreteBinding = binding as ConcreteBinding;
            bool hitCache = concreteBinding.Scope == Constants.BindingScope.Singleton;
            bool isCached = Injector.IsCached(concreteBinding.ImplementationType, concreteBinding.Name);

            if (!hitCache || !isCached) {
                object instance = Creator.CreateInstance(concreteBinding.ImplementationType);
                if (hitCache) {
                    Injector.SetInstance(concreteBinding, instance, concreteBinding.ImplementationType, concreteBinding.Name);
                }

                return instance;
            }

            return Injector.GetInstance(concreteBinding.ImplementationType, concreteBinding.Name);
        }

        void IBindingHandler.Initialize(IBinding binding) {
            // no reason to set bindings against null (which will happen when binding to a type). Just return instead.
            object instance = (binding as ConcreteBinding)?.Instance;
            if (instance == null) {
                return;
            }

            Type instanceType = instance.GetType();
            Injector.SetInstance(binding, (binding as ConcreteBinding)?.Instance, instanceType, binding.Name);
        }
    }
}
