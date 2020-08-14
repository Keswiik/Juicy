using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Injection {
    public interface IInjector {
        T Get<T>();

        T Get<T>(string name);
    }
}
