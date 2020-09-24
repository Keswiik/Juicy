using Juicy.Inject.Binding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject.Code.MappedService {
    public sealed class MappedService1 : IMappedService {
        private readonly int num1;

        [Inject]
        public MappedService1([Named("Num1")] int num1) {
            this.num1 = num1;
        }

        public void DoSomething() {
            Console.WriteLine($"This is MappedService1, received value of num1: {num1}.");
        }
    }
}
