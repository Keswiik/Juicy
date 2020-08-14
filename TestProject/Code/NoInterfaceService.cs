using Juicy.Inject.Binding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject.Code {
    public class NoInterfaceService {
        private string toPrint;

        [Inject]
        public NoInterfaceService([Named("PrintString")] string toPrint) {
            this.toPrint = toPrint;
        }

        public void PrintString() {
            Console.WriteLine($"NoInterfaceService injected string: {toPrint}");
        }
    }
}
