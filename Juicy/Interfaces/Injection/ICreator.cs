using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Injection {
    internal interface ICreator {
        object CreateInstance(Type type);
        object CreateInstanceWithParameters(Type type, params IParameterData[] args);
    }
}
