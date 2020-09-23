using Juicy.Inject.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Constants {

    /// <summary>
    /// The type of binding being used.
    /// </summary>
    public enum BindingType {
        Collection,
        Concrete,
        Factory,
        Method,
        None,
        Provider
    }
}
