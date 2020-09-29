using Juicy.Inject.Injection;
using Juicy.Interfaces.Injection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy {
    public static class Juicer {

        private static ILoggerFactory loggerFactory;

        public static IInjector CreateInjector(params AbstractModule[] modules) {
            return new Injector(loggerFactory, modules);
        }

        public static void AddLogging(ILoggerFactory loggerFactory) {
            Juicer.loggerFactory = loggerFactory;
        }
    }
}
