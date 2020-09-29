namespace Juicy.Inject.Injection.Provide {

    /// <summary>
    /// Exposes unparameterized
    /// </summary>
    internal abstract class Provider {
        protected bool CacheInstance { get; }

        protected bool InstanceCached { get; set; }

        protected Injector Injector { get; }

        /// <summary>
        /// Creates an instance with the specified cache flag and parent injector.
        /// </summary>
        /// <remarks>
        /// Caching is enabled if the provider is for a singleton-scoped item.
        /// </remarks>
        /// <param name="cacheInstance">Whether or not to cache the instance after creating it.</param>
        /// <param name="injector">The injector to use when creating the object.</param>
        protected Provider(bool cacheInstance, Injector injector) {
            CacheInstance = cacheInstance;
            Injector = injector;
        }
    }
}