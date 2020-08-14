using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection.Provide {
    internal class Provider {
        protected bool CacheInstance { get; }

        protected bool InstanceCached { get; set; }

        protected Injector Injector { get; }

        internal Provider(bool cacheInstance, Injector injector) {
            CacheInstance = cacheInstance;
            Injector = injector;
        }
    }
}
