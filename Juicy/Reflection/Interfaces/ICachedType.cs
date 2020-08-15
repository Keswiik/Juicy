﻿using System;
using System.Collections.Generic;

 namespace Juicy.Reflection.Interfaces {

    /// <summary>
    /// Caches basic information about a type.
    /// </summary>
    public interface ICachedType : IAttributeHolder {

        /// <summary>
        /// Cached version's of the types constructors.
        /// </summary>
        List<ICachedMethod> Constructors { get; }

        /// <summary>
        /// Cached versions of the type's methods.
        /// </summary>
        Dictionary<string, List<ICachedMethod>> Methods { get; }

        /// <summary>
        /// The actual type.
        /// </summary>
        Type Type { get; }
    }
}