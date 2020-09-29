namespace Juicy.Interfaces.Injection {

    /// <summary>
    /// A wrapper that enables lazily-loaded access to injectable type at runtime.
    /// </summary>
    /// <typeparam name="T">The type requested by the provider.</typeparam>
    public interface IProvider<T> {

        /// <summary>
        /// Gets an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An instance of <typeparamref name="T"/>, or <c>null</c>.</returns>
        public T Get();
    }
}