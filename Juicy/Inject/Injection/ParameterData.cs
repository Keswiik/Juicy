using Juicy.Interfaces.Injection;

namespace Juicy.Inject.Injection {

    /// <inheritdoc cref="IParameterData"/>
    internal class ParameterData : IParameterData {
        public string Name { get; }

        public object Value { get; }

        internal ParameterData(string name, object value) {
            Name = name;
            Value = value;
        }
    }
}