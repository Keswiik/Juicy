using Juicy.Inject.Injection;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy {
    public static class Juicer {
        public static IInjector CreateInjector(params AbstractModule[] modules) {
            return new Injector(modules);
        }
    }
}
