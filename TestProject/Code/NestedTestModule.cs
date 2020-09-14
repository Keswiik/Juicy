using Juicy.Inject.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject.Code {
    internal class NestedTestModule : AbstractModule {
        public override void Configure() {
            Bind<int>() //
                .Named("Num1") //
                .ToInstance(25);
        }
    }
}
