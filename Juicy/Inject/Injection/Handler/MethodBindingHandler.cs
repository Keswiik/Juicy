using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection.Handler {
    internal class MethodBindingHandler : IBindingHandler {

        private Injector Injector { get; }

        private IMethodInvoker MethodInvoker { get; }

        internal MethodBindingHandler(Injector injector, IMethodInvoker methodInvoker) {
            Injector = injector;
            MethodInvoker = methodInvoker;
        }

        bool IBindingHandler.CanHandle(IBinding binding) {
            return binding is MethodBinding;
        }

        object IBindingHandler.Handle(IBinding binding, Type type, string name) {
            var methodBinding = binding as MethodBinding;
            bool hitCache = methodBinding.Scope == Constants.BindingScope.Singleton;
            bool isCached = Injector.IsCached(methodBinding.BaseType, methodBinding.Name);

            if (!hitCache || !isCached) {
                var instance = MethodInvoker.Invoke(methodBinding.Module, methodBinding.Method);
                if (hitCache) {
                    Injector.SetInstance(methodBinding, instance, type, name);
                }

                return instance;
            }

            return Injector.GetInstance(type, name);
        }

        void IBindingHandler.Initialize(IBinding binding) {
        }

        bool IBindingHandler.NeedsInitialized(IBinding binding) {
            return false;
        }
    }
}
