using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Interfaces.Injection {
    public interface IProvider<T> {
        public T Get();
    }
}
