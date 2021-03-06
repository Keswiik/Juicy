﻿using Juicy.Constants;
using System;
using System.Collections.Generic;

namespace Juicy.Reflection.Interfaces {

    /// <summary>
    /// Cached information about a method, either public or static.
    /// </summary>
    public interface ICachedMethod : IAttributeHolder {

        /// <summary>
        /// The method's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The name of the binding this method is for.
        /// </summary>
        /// <remarks>
        /// This is only applicable for methods marked for injection bindings.
        /// </remarks>
        string BindingName { get; }

        /// <summary>
        /// Whether or not the method provides bindings.
        /// </summary>
        bool Provides { get; }

        /// <summary>
        /// The scope of the binding provided by this method.
        /// </summary>
        /// <remarks>
        /// Defaults to instance bindings, but only matters when methods are marked for injection bindings.
        /// </remarks>
        BindingScope Scope { get; }

        /// <summary>
        /// The method's return type.
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        /// Information about all parameters the method has. It can be empty, but never <c>null</c>.
        /// </summary>
        List<ICachedParameter> Parameters { get; }

        /// <summary>
        /// Calls the method on the specified <paramref name="instance"/> with <paramref name="args"/>.
        /// </summary>
        /// <typeparam name="T">The desired return type of the method.</typeparam>
        /// <param name="instance">The object to call the method on.</param>
        /// <param name="args">Arguments to call the method with.</param>
        /// <returns>An instance of <typeparamref name="T"/>, or <c>null</c>.</returns>
        T Invoke<T>(object instance, params object[] args);

        /// <summary>
        /// Calls the method on the specified <paramref name="instance"/> with <paramref name="args"/>.
        /// </summary>
        /// <remarks>
        /// This method does no casting and returns an <c>object</c>.
        /// </remarks>
        /// <param name="instance">The object to call the method on.</param>
        /// <param name="args">Arguments to call the method with.</param>
        /// <returns>Whatever object was returned by the method, or <c>null</c>.</returns>
        object Invoke(object instance, params object[] args);
    }
}