using System;
using System.Collections.Generic;
using System.Text;

 namespace Juicy.Reflection.Interfaces
{
    public interface ICachedParameter : IAttributeHolder
    {
        Type Type { get; }

        int Position { get; }
    }
}
