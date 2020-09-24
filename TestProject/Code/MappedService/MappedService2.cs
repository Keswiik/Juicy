using Juicy.Inject.Binding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject.Code.MappedService {
    public class MappedService2 : IMappedService {

        private readonly string printString;

        [Inject]
        public MappedService2([Named("PrintString")] string printString) {
            this.printString = printString;
        }

        public void DoSomething() {
            Console.WriteLine($"This is MappedService2, received print string: {printString}.");
        }
    }
}
