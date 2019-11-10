using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Reflector.Interfaces
{
    public interface ICachedMethod : IAttributeHolder
    {
        string Name { get; }

        Type ReturnType { get; }

        List<ICachedParameter> Parameters { get; }

        T Invoke<T>(object instance, params object[] args);

        object Invoke(object instance, params object[] args);
    }
}
