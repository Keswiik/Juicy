using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Binding {
    public interface IMethodBinding : IBinding {
        public ICachedMethod Method { get; }
    }
}
