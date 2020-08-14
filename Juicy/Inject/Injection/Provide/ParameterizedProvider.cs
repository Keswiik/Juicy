using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection.Provide {
    internal class ParameterizedProvider<T> : Provider, IProvider<T> {

        internal ParameterizedProvider(bool cacheInstance, Injector injector) : base(cacheInstance, injector) { }

        protected T Instance { get; set; }

        public T Get() {
            if (InstanceCached) {
                return Instance;
            }

            T instance = Injector.Get<T>();
            if (CacheInstance) {
                Instance = instance;
            }

            return instance;
        }
    }
}
