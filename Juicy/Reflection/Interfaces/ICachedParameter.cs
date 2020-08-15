using System;
using System.Collections.Generic;
using System.Text;

 namespace Juicy.Reflection.Interfaces
{

    /// <summary>
    /// Information about method parameters.
    /// </summary>
    public interface ICachedParameter : IAttributeHolder
    {

        /// <summary>
        /// The type of the parameter.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// The position of the parameter.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        /// <remarks>
        /// Only applicable for parameters that are used in named injections.
        /// </remarks>
        string Name { get; }
    }
}
