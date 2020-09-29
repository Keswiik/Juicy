using System;
using System.Collections.Generic;

namespace Juicy.Reflection.Interfaces {

    /// <summary>
    /// A container used to provide access to attributes on an item.
    /// </summary>
    public interface IAttributeHolder {

        /// <summary>
        /// Checks if the container has an attribute of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The attribute type to check for.</param>
        /// <returns><c>true</c> if the container has an instance of the attribute, otherwise <c>false</c>.</returns>
        bool HasAttribute(Type type);

        /// <summary>
        /// Gets an attribute of <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// If the attribute can be used multiple times, do <b>not</b> use this. Use <see cref="GetAttributes{T}"/> instead.
        /// </remarks>
        /// <typeparam name="T">The type of attribute to get.</typeparam>
        /// <returns>An instance of <typeparamref name="T"/>, or <c>null</c> if the attribute isn't present.</returns>
        T GetAttribute<T>() where T : Attribute;

        /// <summary>
        /// Gets multiple attributes of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The attribute type to find.</typeparam>
        /// <returns>A list of <typeparamref name="T"/> attributes which may be empty.</returns>
        List<T> GetAttributes<T>() where T : Attribute;

        /// <summary>
        /// Gets an attribute of <paramref name="type"/>.
        /// </summary>
        /// <remarks>
        /// If the attribute can be used multiple times, do <b>not</b> use this. Use <see cref="GetAttributes{T}"/> instead.
        /// </remarks>
        /// <param name="type">The type of attribute to find.</param>
        /// <returns>An instance of <typeparamref name="T"/>, or <c>null</c> if the attribute isn't present.</returns>
        Attribute GetAttribute(Type type);

        /// <summary>
        /// Gets multiple attributes of <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type of attribute to find.</param>
        /// <returns>A list of <paramref name="type"/> attributes which may be empty.</returns>
        List<Attribute> GetAttributes(Type type);
    }
}