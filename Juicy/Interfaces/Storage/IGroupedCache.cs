using System;
using System.Collections.Generic;
using System.Text;

 namespace Juicy.Interfaces.Storage {

    /// <summary>
    /// A cache that stores both a default value and keyed values of the same type.
    /// </summary>
    /// <remarks>This is used to provide unnamed (default) and keyed (named) storage of bindings, instances, and providers.</remarks>
    /// <typeparam name="T">The type of value being stored.</typeparam>
    /// <typeparam name="K">The type of key used to retrieve non-default values.</typeparam>
    internal interface IGroupedCache<T, K> {

        /// <summary>
        /// Gets the default value of the group.
        /// </summary>
        /// <returns>An instance of <typeparamref name="T"/>, or <c>null</c> if there is no default value.</returns>
        internal T Get();

        /// <summary>
        /// Gets a keyed value from the group.
        /// </summary>
        /// <param name="key">The key to retrieve values with.</param>
        /// <returns>An instance of <typeparamref name="T"/>, or <c>null</c>.</returns>
        internal T Get(K key);

        /// <summary>
        /// Caches the default value of the group.
        /// </summary>
        /// <param name="value">The default value to cache.</param>
        internal void Cache(T value);

        /// <summary>
        /// Caches a keyed value in the group.
        /// </summary>
        /// <param name="value">The value to cache.</param>
        /// <param name="key">The key to cache the value with.</param>
        internal void Cache(T value, K key);

        /// <summary>
        /// Checks if a value is cached under <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to check for cached values with.</param>
        /// <returns><c>true</c> if the value is cached, otherwise <c>false</c>.</returns>
        internal bool IsCached(K key);

        /// <summary>
        /// Checks if a default value is cached.
        /// </summary>
        /// <returns><c>true</c> if a default value is cached, otherwise <c>false</c>.</returns>
        internal bool IsCached();
    }
}
