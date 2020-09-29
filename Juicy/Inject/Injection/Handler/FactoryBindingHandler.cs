using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Juicy.Inject.Injection.Handler {

    /// <summary>
    /// Handler used to create injections based on <see cref="FactoryBinding"/>.
    /// </summary>
    internal sealed class FactoryBindingHandler : AbstractBindingHander {
        private readonly static MethodInfo CreateProxyMethod = typeof(DispatchProxy).GetMethod("Create");

        private readonly ICreator creator;

        internal FactoryBindingHandler(Injector injector, ILoggerFactory loggerFactory, ICreator creator) : base(injector, loggerFactory) {
            this.creator = creator;
        }

        public override object Handle(IBinding binding, Type type, string name) {
            var factoryBinding = binding as FactoryBinding;
            bool isCached = Injector.IsCached(factoryBinding.BaseType, factoryBinding.Name);
            if (!isCached) {
                logger?.LogTrace("Creating factory proxy for {Binding}.", factoryBinding);
                var proxy = MakeProxy(factoryBinding.BaseType, factoryBinding.ImplementationType);
                Injector.SetInstance(factoryBinding, proxy, factoryBinding.BaseType, factoryBinding.Name);

                return proxy;
            }

            return Injector.GetInstance(factoryBinding.BaseType, factoryBinding.Name);
        }

        private object MakeProxy(Type proxyType, Type resultType) {
            // the proxy is able to mimic proxyType at runtime, which will be the injected value
            FactoryProxy instance = (FactoryProxy)CreateProxyMethod.MakeGenericMethod(proxyType, typeof(FactoryProxy)) //
                .Invoke(null, new object[] { });
            instance.Creator = creator;
            instance.ResultType = resultType;

            return instance;
        }
    }
}
