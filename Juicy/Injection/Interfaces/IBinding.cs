using Juicy.Injection.Bindings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Injection.Interfaces
{
    internal interface IBinding
    {
        Type BaseType { get; }

        Type ImplementationType { get; set; }

        Scope Scope { get; set; }

        object Instance { get; set; }

        string Name { get; set; }
    }
}
