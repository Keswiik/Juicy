using Juicy.Constants;
using Juicy.Inject.Binding;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Juicy.Reflection;
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

        private static readonly IMethodBindingFactory methodBindingFactory = new MethodBindingFactory(new Reflector());

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

        public DictionaryBinding.DictionaryBindingBuilder BindDictionary<T>() where T : IDictionary {
            var builder = new DictionaryBinding.DictionaryBindingBuilder(typeof(T), BindingType.Dictionary, this);
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

        public AbstractModule Override(AbstractModule module) {
            return new OverrideModule(this, module);
        }

        /// <summary>
        /// All explicit bindings using <see cref="Bind{T}"/>, <see cref="BindFactory{T}"/>, or <see cref="BindMany{T}"/> should be added in the override of this method.
        /// </summary>
        /// <remarks>
        /// The default implementation does nothing. Not all consumers may need to use this method.
        /// </remarks>
        public virtual void Configure() { }

        virtual internal List<IBinding> GetBindings() {
            Configure();

            List<IBinding> bindings = new List<IBinding>();
            bindings.AddRange(methodBindingFactory.CreateBindings(this));
            foreach (IBuilder builder in BindingBuilders) {
                bindings.Add(builder.Build());
            }
            foreach (AbstractModule module in InstalledModules) {
                bindings.AddRange(module.GetBindings());
            }


            return bindings;
        }
    }
}
