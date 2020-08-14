using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Binding {
    public interface IFactoryBinding : IBinding {

        public Type GenericType { get; }

        public Type ImplementationType { get; }
    }
}
