using Juicy.Interfaces.Storage;
using System;
using System.Collections.Generic;

 namespace Juicy.Inject.Storage {
    internal class Cache<T, K, N> : ICache<T, K, N> {

        private Dictionary<K, IGroupedCache<T, N>> Dictionary { get; }

        internal Cache() {
            Dictionary = new Dictionary<K, IGroupedCache<T, N>>();
        }

        void ICache<T, K, N>.Cache(T value, K key) {
            var groupedCache = GetGroupedCache(key);
            if (groupedCache == null) {
                groupedCache = CreateAndStore(key);
            }

            if (groupedCache.IsCached()) {
                throw new InvalidOperationException($"Cache already contains an instance of {value}");
            }

            groupedCache.Cache(value);
        }

        void ICache<T, K, N>.Cache(T value, K key, N subKey) {
            var groupedCache = GetGroupedCache(key);
            if (groupedCache == null) {
                groupedCache = CreateAndStore(key);
            }

            if (groupedCache.IsCached(subKey)) {
                throw new InvalidOperationException($"Cache already contains an instance of {value} with sub-key {subKey}");
            }

            groupedCache.Cache(value, subKey);
        }

        T ICache<T, K, N>.Get(K key) {
            var groupedCache = GetGroupedCache(key);

            return groupedCache != null ? groupedCache.Get() : default;
        }

        T ICache<T, K, N>.Get(K key, N subKey) {
            var groupedCache = GetGroupedCache(key);

            return groupedCache != null ? groupedCache.Get(subKey) : default;
        }

        bool ICache<T, K, N>.IsCached(K key) {
            var groupedCache = GetGroupedCache(key);

            return groupedCache?.IsCached() == true;
        }

        bool ICache<T, K, N>.IsCached(K key, N subKey) {
            var groupedCache = GetGroupedCache(key);

            return groupedCache?.IsCached(subKey) == true;
        }

        private IGroupedCache<T, N> GetGroupedCache(K key) {
            Dictionary.TryGetValue(key, out IGroupedCache<T, N> groupedCache);
            return groupedCache;
        }

        private IGroupedCache<T, N> CreateAndStore(K key) {
            var groupedCache = new GroupedCache<T, N>();
            Dictionary[key] = groupedCache;
            return groupedCache;
        }
    }
}
