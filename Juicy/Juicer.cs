using Juicy.Inject.Injection;
using Juicy.Interfaces.Injection;
using Microsoft.Extensions.Logging;

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