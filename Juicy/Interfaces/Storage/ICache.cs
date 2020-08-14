using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

 namespace Juicy.Interfaces.Storage {
    internal interface ICache<T, K, N> {
        internal bool IsCached(K key);
        internal bool IsCached(K key, N subKey);
        internal T Get(K key);
        internal T Get(K key, N subKey);
        internal void Cache(T value, K key);
        internal void Cache(T value, K key, N subKey);
    }
}
