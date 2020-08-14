using System;
using System.Collections.Generic;
using System.Text;

 namespace Juicy.Interfaces.Storage {
    internal interface IGroupedCache<T, K> {
        internal T Get();
        internal T Get(K key);
        internal void Cache(T value);
        internal void Cache(T value, K key);
        internal bool IsCached(K key);
        internal bool IsCached();
    }
}
