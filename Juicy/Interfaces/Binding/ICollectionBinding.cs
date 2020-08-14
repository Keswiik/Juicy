using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Binding {
    public interface ICollectionBinding : IBinding {

        public List<Type> ImplementationTypes { get; }
    }
}
