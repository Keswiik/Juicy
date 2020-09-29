using Juicy.Interfaces.Storage;
using System;
using System.Collections.Generic;

 namespace Juicy.Inject.Storage {

    /// <inheritdoc cref="ICache{T, K, N} "/>
    internal sealed class InMemoryCache<T, K, N> : ICache<T, K, N> {

        private Dictionary<K, IGroupedCache<T, N>> Dictionary { get; }

        internal InMemoryCache() {
            Dictionary = new Dictionary<K, IGroupedCache<T, N>>();
        }

        public void Cache(T value, K key) {
            var groupedCache = GetGroupedCache(key);
            if (groupedCache == null) {
                groupedCache = CreateAndStore(key);
            }

            if (groupedCache.IsCached()) {
                throw new InvalidOperationException($"Cache already contains an instance of {value}");
            }

            groupedCache.Cache(value);
        }

        public void Cache(T value, K key, N subKey) {
            var groupedCache = GetGroupedCache(key) ?? CreateAndStore(key);

            if (groupedCache.IsCached(subKey)) {
                throw new InvalidOperationException($"Cache already contains an instance of {value} with sub-key {subKey}");
            }

            groupedCache.Cache(value, subKey);
        }

        public T Get(K key) {
            var groupedCache = GetGroupedCache(key);

            return groupedCache != null ? groupedCache.Get() : default;
        }

        public T Get(K key, N subKey) {
            var groupedCache = GetGroupedCache(key);

            return groupedCache != null ? groupedCache.Get(subKey) : default;
        }

        public bool IsCached(K key) {
            return GetGroupedCache(key)?.IsCached() == true;
        }

        public bool IsCached(K key, N subKey) {
            return GetGroupedCache(key)?.IsCached(subKey) == true;
        }

        private IGroupedCache<T, N> GetGroupedCache(K key) {
            Dictionary.TryGetValue(key, out IGroupedCache<T, N> groupedCache);
            return groupedCache;
        }

        private IGroupedCache<T, N> CreateAndStore(K key) {
            var groupedCache = new InMemoryGroupedCache<T, N>();
            Dictionary[key] = groupedCache;
            return groupedCache;
        }
    }
}
