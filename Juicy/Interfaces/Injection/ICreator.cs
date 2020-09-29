using System;

namespace Juicy.Interfaces.Injection {

    /// <summary>
    /// Creates new instances of classes during the injection process.
    /// </summary>
    internal interface ICreator {

        /// <summary>
        /// Create an instance of <paramref name="type"/> without pre-existing arguments.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <returns>An instance of <paramref name="type"/>.</returns>
        object CreateInstance(Type type);

        /// <summary>
        /// Create an instance of <paramref name="type"/> with pre-existing arguments.
        /// </summary>
        /// <remarks>
        /// Primarily used when factories that accept arguments are called. It will attempt to match the provided parameter data to arguments present within an injectable constructor.
        /// </remarks>
        /// <param name="type">The type to create.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>An instance of <paramref name="type"/>.</returns>
        object CreateInstanceWithParameters(Type type, params IParameterData[] args);
    }
}