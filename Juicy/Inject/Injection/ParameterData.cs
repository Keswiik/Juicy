using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection {

    /// <inheritdoc cref="IParameterData"/>
    internal class ParameterData : IParameterData {
        string IParameterData.Name => Name;

        object IParameterData.Value => Value;

        private string Name { get; }

        private object Value { get; }

        internal ParameterData(string name, object value) {
            Name = name;
            Value = value;
        }
    }
}
