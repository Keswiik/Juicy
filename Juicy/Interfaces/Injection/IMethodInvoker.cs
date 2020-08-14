using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Injection {
    interface IMethodInvoker {
        internal object Invoke(object instance, ICachedMethod method);
    }
}
