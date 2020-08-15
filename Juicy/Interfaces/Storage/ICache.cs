using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

 namespace Juicy.Interfaces.Storage {

    /// <summary>
    /// Caches values using a <typeparamref name="K"/> primary key and <typeparamref name="N"/> subkey.
    /// </summary>
    /// <typeparam name="T">The type to cache.</typeparam>
    /// <typeparam name="K">The key to cache values with.</typeparam>
    /// <typeparam name="N">The subkey to cache values with.</typeparam>
    internal interface ICache<T, K, N> {

        /// <summary>
        /// Checks if the cache contains an unnamed value for the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to check for cached values with.</param>
        /// <returns><c>true</c> if the <paramref name="key"/> is cached, otherwise <c>false</c>.</returns>
        internal bool IsCached(K key);

        /// <summary>
        /// Checks if the cache contains an unnamed value for the specified <paramref name="key"/> and <paramref name="subKey"/>.
        /// </summary>
        /// <param name="key">The key to check for cached values with.</param>
        /// <param name="subKey">The subkey to check for cached values with.</param>
        /// <returns><c>true</c> if the <paramref name="key"/> is cached, otherwise <c>false</c>.</returns>
        internal bool IsCached(K key, N subKey);

        /// <summary>
        /// Gets a value from the cache using only a <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to get cached values with.</param>
        /// <returns>An instance of <typeparamref name="T"/>, or <c>null</c> if no value exists.</returns>
        internal T Get(K key);

        /// <summary>
        /// Gets a value from the cache using both a <paramref name="key"/> and <paramref name="subKey"/>.
        /// </summary>
        /// <param name="key">The key to get cached values with.</param>
        /// <param name="subKey">The subkey to get cached values with.</param>
        /// <returns>An object of type <typeparamref name="T"/>, or <c>null</c> if no value exists.</returns>
        internal T Get(K key, N subKey);

        /// <summary>
        /// Caches a value with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="value">The value to cache.</param>
        /// <param name="key">The key to cache the value with.</param>
        internal void Cache(T value, K key);

        /// <summary>
        /// Caches a value with the specified <paramref name="key"/> and <paramref name="subKey"/>.
        /// </summary>
        /// <param name="value">The value to cache.</param>
        /// <param name="key">The key to cache the value with.</param>
        /// <param name="subKey">The subkey to cache the value with.</param>
        internal void Cache(T value, K key, N subKey);
    }
}
