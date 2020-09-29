using Juicy.Interfaces.Storage;
using System.Collections.Generic;

 namespace Juicy.Inject.Storage {
    
    /// <inheritdoc cref="IGroupedCache{T, K}"/>
    internal sealed class InMemoryGroupedCache<T, K> : IGroupedCache<T, K> {
        
        private Dictionary<K, T> Dictionary { get; }

        private T DefaultValue { get; set; }

        private bool DefaultCached { get; set; }

        internal InMemoryGroupedCache() {
            Dictionary = new Dictionary<K, T>();
        }

        public void Cache(T value) {
            DefaultValue = value;
            DefaultCached = true;
        }

        public void Cache(T value, K key) {
            Dictionary[key] = value;
        }

        public T Get() {
            return DefaultValue;
        }

        public T Get(K key) {
            return Dictionary.ContainsKey(key) ? Dictionary[key] : default;
        }

        bool IGroupedCache<T, K>.IsCached(K key) {
            return Dictionary.ContainsKey(key);
        }

        bool IGroupedCache<T, K>.IsCached() {
            return DefaultCached;
        }
    }
}
