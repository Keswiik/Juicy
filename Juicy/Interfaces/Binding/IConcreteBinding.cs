using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Binding {
    public interface IConcreteBinding : IBinding {
        public Type ImplementationType { get; }
        public object Instance { get; }
    }
}
