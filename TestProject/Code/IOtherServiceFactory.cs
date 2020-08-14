using Juicy.Inject.Binding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject.Code {
    public interface IOtherServiceFactory {
        public IOtherService CreateService([Named("Num2")] int number);
    }
}
