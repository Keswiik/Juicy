using System;
using System.Collections.Generic;

namespace Juicy.Reflector.Interfaces {

    public interface ICachedType : IAttributeHolder {
        List<ICachedMethod> Constructors { get; }

        Dictionary<string, List<ICachedMethod>> Methods { get; }

        Type Type { get; }
    }
}