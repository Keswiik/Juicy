using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Reflector.Interfaces
{
    public interface IAttributeHolder
    {
        bool HasAttribute(Type type);

        T GetAttribute<T>() where T : Attribute;

        List<T> GetAttributes<T>() where T : Attribute;

        Attribute GetAttribute(Type type);

        List<Attribute> GetAttributes(Type type);
    }
}
