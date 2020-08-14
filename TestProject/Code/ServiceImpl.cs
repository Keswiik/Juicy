using Juicy.Inject.Binding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject.Code {
    internal sealed class ServiceImpl : IService {

        private readonly int number;

        [Inject]
        public ServiceImpl([Named("Num1")] int number) {
            this.number = number;
        }

        public void DoThing() {
            Console.WriteLine("Inside of ServiceImpl");
        }

        public int GetNumber() {
            return number;
        }
    }
}
