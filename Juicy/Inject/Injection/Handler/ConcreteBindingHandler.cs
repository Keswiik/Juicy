using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection.Handler {
    internal class ConcreteBindingHandler : IBindingHandler {

        private Injector Injector { get; }

        private ICreator Creator { get; }

        internal ConcreteBindingHandler(Injector injector, ICreator creator) {
            Injector = injector;
            Creator = creator;
        }

        bool IBindingHandler.CanHandle(IBinding binding) {
            return binding is ConcreteBinding;
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

        bool IBindingHandler.NeedsInitialized(IBinding binding) {
            var concreteBinding = binding as ConcreteBinding;
            return concreteBinding.Instance != null;
        }

        void IBindingHandler.Initialize(IBinding binding) {
            object instance = (binding as ConcreteBinding).Instance;
            Type instanceType = instance.GetType();
            Injector.SetInstance(binding, (binding as ConcreteBinding)?.Instance, instanceType, binding.Name);
        }
    }
}
