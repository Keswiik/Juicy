using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection.Handler {

    /// <summary>
    /// Handler used to create injections based on <see cref="MethodBinding"/>.
    /// </summary>
    internal sealed class MethodBindingHandler : AbstractBindingHander {

        private IMethodInvoker MethodInvoker { get; }

        /// <summary>
        /// Creates a new handler with the specified parent injector and method invoker.
        /// </summary>
        /// <param name="injector">The parent injector to use.</param>
        /// <param name="methodInvoker">The method invoker to use.</param>
        internal MethodBindingHandler(Injector injector, ILoggerFactory loggerFactory, IMethodInvoker methodInvoker) : base(injector, loggerFactory) {
            MethodInvoker = methodInvoker;
        }

        public override object Handle(IBinding binding, Type type, string name) {
            var methodBinding = binding as MethodBinding;
            bool hitCache = methodBinding.Scope == Constants.BindingScope.Singleton;
            bool isCached = Injector.IsCached(methodBinding.BaseType, methodBinding.Name);

            if (!hitCache || !isCached) {
                logger?.LogTrace("Creating implementation for {Binding}.", methodBinding);
                var instance = MethodInvoker.Invoke(methodBinding.Module, methodBinding.Method);
                if (hitCache) {
                    logger?.LogTrace("Caching implementation for {Binding}.", methodBinding);
                    Injector.SetInstance(methodBinding, instance, type, name);
                }

                return instance;
            }

            return Injector.GetInstance(type, name);
        }
    }
}
