using System;
using System.Collections.Generic;

 namespace Juicy.Reflection.Interfaces {

    public interface ICachedType : IAttributeHolder {
        List<ICachedMethod> Constructors { get; }

        Dictionary<string, List<ICachedMethod>> Methods { get; }

        Type Type { get; }
    }
}