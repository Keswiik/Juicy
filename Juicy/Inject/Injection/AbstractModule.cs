using Juicy.Constants;
using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace Juicy.Inject.Injection {

    /// <summary>
    /// Base module all consumers should inherit from.
    /// </summary>
    public abstract class AbstractModule : IModule {

        internal List<IBuilder> BindingBuilders { get; }

        internal List<AbstractModule> InstalledModules { get; }

        protected AbstractModule() {
            BindingBuilders = new List<IBuilder>();
            InstalledModules = new List<AbstractModule>();
        }

        public ConcreteBinding.ConcreteBindingBuilder Bind<T>() {
            var builder = new ConcreteBinding.ConcreteBindingBuilder(typeof(T), BindingType.Concrete, this);
            BindingBuilders.Add(builder);
            return builder;
        }

        public FactoryBinding.FactoryBindingBuilder BindFactory<T>() {
            var builder =  new FactoryBinding.FactoryBindingBuilder(typeof(T), BindingType.Factory, this);
            BindingBuilders.Add(builder);
            return builder;
        }

        public CollectionBinding.CollectionBindingBuilder BindMany<T>() where T : IEnumerable {
            var builder =  new CollectionBinding.CollectionBindingBuilder(typeof(T), BindingType.Collection, this);
            BindingBuilders.Add(builder);
            return builder;
        }

        public void Install(IModule module) {
            if (!(module is AbstractModule)) {
                throw new InvalidOperationException("Installed module is not an AbstractModule.");
            }

            InstalledModules.Add(module as AbstractModule);
        }

        /// <summary>
        /// All explicit bindings using <see cref="Bind{T}"/>, <see cref="BindFactory{T}"/>, or <see cref="BindMany{T}"/> should be added in the override of this method.
        /// </summary>
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
