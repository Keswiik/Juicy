namespace Juicy.Interfaces.Binding {

    /// <summary>
    /// Allows for bindings to be built.
    /// </summary>
    /// <remarks>
    /// Really, this is only here to expose the <see cref="IBuilder.Build()"/> method to injectors.
    /// </remarks>
    public interface IBuilder {

        /// <summary>
        /// Builds a new binding.
        /// </summary>
        /// <returns>The new binding.</returns>
        internal IBinding Build();
    }
}