using Juicy.Inject.Binding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using TestProject.Code.Attributes;

namespace TestProject.Code {
    public class NoInterfaceService {
        private string toPrint;

        [Inject]
        public NoInterfaceService([PrintString] string toPrint) {
            this.toPrint = toPrint;
        }

        public void PrintString() {
            Console.WriteLine($"NoInterfaceService injected string: {toPrint}");
        }
    }
}
