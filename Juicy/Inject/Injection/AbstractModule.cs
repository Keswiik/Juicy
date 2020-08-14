using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection {
    public abstract class AbstractModule : IModule {

        internal List<IBuilder> BindingBuilders { get; }

        protected AbstractModule() {
            BindingBuilders = new List<IBuilder>();
        }

        public ConcreteBinding.ConcreteBindingBuilder Bind<T>() {
            var builder = new ConcreteBinding.ConcreteBindingBuilder(typeof(T), this);
            BindingBuilders.Add(builder);
            return builder;
        }

        public FactoryBinding.FactoryBindingBuilder BindFactory<T>() {
            var builder =  new FactoryBinding.FactoryBindingBuilder(typeof(T), this);
            BindingBuilders.Add(builder);
            return builder;
        }

        public CollectionBinding.CollectionBindingBuilder BindMany<T>() where T : IEnumerable {
            var builder =  new CollectionBinding.CollectionBindingBuilder(typeof(T), this);
            BindingBuilders.Add(builder);
            return builder;
        }

        public abstract void Configure();

        internal List<IBinding> GetBindings() {
            List<IBinding> bindings = new List<IBinding>();
            foreach (IBuilder builder in BindingBuilders) {
                bindings.Add(builder.Build());
            }

            return bindings;
        }
    }
}
