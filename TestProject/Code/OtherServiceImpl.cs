using Juicy.Inject.Binding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject.Code {
    public class OtherServiceImpl : IOtherService {

        private readonly int number1;

        private readonly int number2;

        [Inject]
        public OtherServiceImpl([Named("Num1")] int number1, [Named("Num2")] int number2) {
            this.number1 = number1;
            this.number2 = number2;
        }

        public int GetNumber1() {
            return number1;
        }

        public int GetNumber2() {
            return number2;
        }
    }
}
