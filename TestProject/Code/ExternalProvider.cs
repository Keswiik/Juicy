using Juicy.Inject.Binding.Attributes;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject.Code {
    public class ExternalProvider : IProvider<IExternallyProvidedService> {

        private readonly string printString;

        [Inject]
        public ExternalProvider([Named("PrintString")] string printString) {
            this.printString = printString;
        }

        public IExternallyProvidedService Get() {
            return new ExternallyProvidedService(printString);
        }
    }
}
