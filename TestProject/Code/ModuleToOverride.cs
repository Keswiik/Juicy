using Juicy.Constants;
using Juicy.Inject.Binding.Attributes;
using Juicy.Inject.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject.Code {
    internal class ModuleToOverride : AbstractModule {
        public override void Configure() {
        }

        [Provides]
        [Scope(BindingScope.Singleton)]
        [Named("PrintString")]
        public string ProvidePrintString() {
            return "This string should be overridden!";
        }
    }
}
