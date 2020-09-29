using Juicy.Interfaces.Injection;

namespace Juicy.Inject.Injection.Provide {

    /// <summary>
    /// Internal implementation of <see cref="IProvider{T}"/> used to pass requests back into the injector.
    /// </summary>
    /// <typeparam name="T">The type of value being provided during injection.</typeparam>
    internal class ParameterizedProvider<T> : Provider, IProvider<T> {
        private T Instance { get; set; }

        /// <inheritdoc/>
        internal ParameterizedProvider(bool cacheInstance, Injector injector) : base(cacheInstance, injector) { }

        public T Get() {
            if (InstanceCached) {
                return Instance;
            }

            T instance = Injector.Get<T>();
            if (CacheInstance) {
                Instance = instance;
                InstanceCached = true;
            }

            return instance;
        }
    }
}