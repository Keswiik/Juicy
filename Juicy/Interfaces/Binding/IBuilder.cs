using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Binding {
    public interface IBuilder {
        internal IBinding Build();
    }
}
