using Juicy.Constants;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Binding {
    public interface IBinding {
        public Type BaseType { get; }

        public BindingScope Scope { get; }

        public string Name { get; }

        public IModule Module { get; }
    }
}
