using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Exceptions {

    /// <summary>
    /// Exception thrown when an error occurs during injection.
    /// </summary>
    public sealed class InjectionException : InvalidOperationException {
        public InjectionException() {
        }

        public InjectionException(string message) : base(message) {
        }

        public InjectionException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
