namespace Juicy.Reflection.Interfaces {

    /// <summary>
    /// Allows building of an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to build.</typeparam>
    internal interface IBuilder<T>
            where T : class {

        /// <summary>
        /// Builds an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        T Build();
    }
}