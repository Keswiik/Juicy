using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Injection {
    internal interface IParameterData {
        internal string Name { get; }
        internal object Value { get; }
    }
}
