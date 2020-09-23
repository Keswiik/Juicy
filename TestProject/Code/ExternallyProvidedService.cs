using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject.Code {
    internal class ExternallyProvidedService : IExternallyProvidedService {

        private readonly string printString;

        public ExternallyProvidedService(string printString) {
            this.printString = printString;
        }

        public string GetPrintString() {
            return printString;
        }
    }
}
