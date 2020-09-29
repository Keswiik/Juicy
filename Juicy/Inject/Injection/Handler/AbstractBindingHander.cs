using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Microsoft.Extensions.Logging;
using System;

namespace Juicy.Inject.Injection.Handler {

    internal abstract class AbstractBindingHander : IBindingHandler {
        protected Injector Injector { get; set; }

        protected readonly ILogger logger;

        protected AbstractBindingHander(Injector injector, ILoggerFactory loggerFactory) {
            Injector = injector;
            logger = loggerFactory?.CreateLogger(GetType().Name);
        }

        public abstract object Handle(IBinding binding, Type type, string name);

        public virtual void Initialize(IBinding binding) {
        }
    }
}