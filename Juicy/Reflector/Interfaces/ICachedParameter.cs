using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Reflector.Interfaces
{
    public interface ICachedParameter : IAttributeHolder
    {
        Type Type { get; }

        int Position { get; }
    }
}
