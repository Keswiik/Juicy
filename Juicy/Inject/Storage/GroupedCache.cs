using Juicy.Interfaces.Storage;
using System.Collections.Generic;

 namespace Juicy.Inject.Storage {
    internal class GroupedCache<T, K> : IGroupedCache<T, K> {
        
        private Dictionary<K, T> Dictionary { get; }
        private T DefaultValue { get; set; }
        private bool DefaultCached { get; set; }

        internal GroupedCache() {
            Dictionary = new Dictionary<K, T>();
        }

        void IGroupedCache<T, K>.Cache(T value) {
            DefaultValue = value;
            DefaultCached = true;
        }

        void IGroupedCache<T, K>.Cache(T value, K key) {
            Dictionary[key] = value;
        }

        T IGroupedCache<T, K>.Get() {
            return DefaultValue;
        }

        T IGroupedCache<T, K>.Get(K key) {
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
