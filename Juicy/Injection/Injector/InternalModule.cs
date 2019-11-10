using Juicy.Injection.Bindings;
using Juicy.Injection.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Injection.Injector {
    internal abstract class InternalModule : IModule {

        protected List<InternalModule> dependentModules;

        protected List<IBinding> bindings;

        protected List<IFactoryBuilder> factories;

        public InternalModule() {
            dependentModules = new List<InternalModule>();
            bindings = new List<IBinding>();
            factories = new List<IFactoryBuilder>();
        }

        internal List<IBinding> GetBindings() {
            return default;
        }

        internal List<InternalModule> GetInternalModules() {
            return default;
        }

        internal List<IFactoryBuilder> GetFactories() {
            return default;
        }

        public IBindingBuilder Bind(Type baseType) {
            IBinding binding = new Binding(baseType);
            bindings.Add(binding);

            return new BindingBuilder(binding);
        }

        public void Install(IModule module) {
            dependentModules.Add((InternalModule)module);
        }

        public void Install(IFactoryBuilder factoryBuilder) {
            factories.Add(factoryBuilder);
        }

        public abstract void Configure();
    }
}
