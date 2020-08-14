using Juicy.Constants;
using Juicy.Inject.Injection.Provide;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Juicy.Inject.Injection.Handler {
    internal class NoBindingHandler : IBindingHandler {
        private readonly static Type ProviderType = typeof(IProvider<>);
        private readonly static Type ParameterizedProviderType = typeof(ParameterizedProvider<>);
        private const BindingFlags ProviderBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        private Injector Injector { get; }

        private ICreator Creator { get; }

        internal NoBindingHandler(Injector injector, ICreator creator) {
            Injector = injector;
            Creator = creator;
        }

        bool IBindingHandler.CanHandle(IBinding binding) {
            return binding == null;
        }

        object IBindingHandler.Handle(IBinding binding, Type type, string name) {
            if (IsProvider(type)) {
                var underlyingType = type.GetGenericArguments()[0];
                binding = Injector.GetBinding(underlyingType, name);

                if (!Injector.ProviderCache.IsCached(type)) {
                    var cacheInstance = binding?.Scope == BindingScope.Singleton;
                    var provider = (Provider)Activator.CreateInstance(ParameterizedProviderType //
                        .MakeGenericType(underlyingType), ProviderBindingFlags, null, new object[] { cacheInstance, this }, null);
                    if (name != null) {
                        Injector.ProviderCache.Cache(provider, type, name);
                    } else {
                        Injector.ProviderCache.Cache(provider, type);
                    }
                }

                return (name != null) ? //
                    Injector.ProviderCache.Get(type, name) : //
                    Injector.ProviderCache.Get(type);
            } else if (!type.IsInterface && !type.IsAbstract && !type.IsValueType) {
                return Creator.CreateInstance(type);
            } else {
                throw new InvalidOperationException($"Cannot create an instance of type {type.FullName} that has no binding");
            }
        }

        void IBindingHandler.Initialize(IBinding binding) {
        }

        bool IBindingHandler.NeedsInitialized(IBinding binding) {
            return false;
        }

        private bool IsProvider(Type type) {
            return type.IsGenericType && type.GetGenericTypeDefinition() == ProviderType;
        }
    }
}
