using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Exceptions {

    /// <summary>
    /// Exception thrown when an invalid binding is detected.
    /// </summary>
    public sealed class InvalidBindingException : InvalidOperationException {
        public InvalidBindingException() {
        }

        public InvalidBindingException(string message) : base(message) {
        }

        public InvalidBindingException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
