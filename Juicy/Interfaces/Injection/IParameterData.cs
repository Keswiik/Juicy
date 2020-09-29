namespace Juicy.Interfaces.Injection {

    /// <summary>
    /// Associates names to provided values during factory-based injection.
    /// </summary>
    internal interface IParameterData {

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The value of the parameter.
        /// </summary>
        object Value { get; }
    }
}