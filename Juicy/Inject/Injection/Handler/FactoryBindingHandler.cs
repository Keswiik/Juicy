using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Juicy.Inject.Injection.Handler {
    internal class FactoryBindingHandler : IBindingHandler {
        private readonly static MethodInfo CreateProxyMethod = typeof(DispatchProxy).GetMethod("Create");

        private ICreator Creator { get; }

        private Injector Injector { get; }

        internal FactoryBindingHandler(Injector injector, ICreator creator) {
            Injector = injector;
            Creator = creator;
        }

        bool IBindingHandler.CanHandle(IBinding binding) {
            return binding is FactoryBinding;
        }

        object IBindingHandler.Handle(IBinding binding, Type type, string name) {
            var factoryBinding = binding as FactoryBinding;
            bool isCached = Injector.IsCached(factoryBinding.BaseType, factoryBinding.Name);
            if (!isCached) {
                var proxy = MakeProxy(factoryBinding.BaseType, factoryBinding.ImplementationType);
                Injector.SetInstance(factoryBinding, proxy, factoryBinding.BaseType, factoryBinding.Name);

                return proxy;
            }

            return Injector.GetInstance(factoryBinding.BaseType, factoryBinding.Name);
        }

        void IBindingHandler.Initialize(IBinding binding) {
        }

        bool IBindingHandler.NeedsInitialized(IBinding binding) {
            return false;
        }

        private object MakeProxy(Type proxyType, Type resultType) {
            FactoryProxy instance = (FactoryProxy)CreateProxyMethod.MakeGenericMethod(proxyType, typeof(FactoryProxy)) //
                .Invoke(null, new object[] { });
            instance.Creator = Creator;
            instance.ResultType = resultType;

            return instance;
        }
    }
}
