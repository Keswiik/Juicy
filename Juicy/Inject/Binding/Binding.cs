using Juicy.Constants;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;

namespace Juicy.Inject.Binding {

    public class Binding : IBinding {
        public Type BaseType { get; }

        public BindingScope Scope { get; }

        public string Name { get; }

        public IModule Module { get; }

        protected Binding(IBindingBuilderComponent builderComponent) {
            BaseType = builderComponent.BaseType;
            Scope = builderComponent.BindingScope;
            Name = builderComponent.Name;
            Module = builderComponent.Module;
        }

        public interface IBindingBuilderComponent {
            internal Type BaseType { get; }

            internal IModule Module { get; }

            internal BindingScope BindingScope { get; }

            internal string Name { get; }
        }

        public abstract class BindingBuilderComponent<T> : IBindingBuilderComponent where T : class, IBindingBuilderComponent {
            Type IBindingBuilderComponent.BaseType => BaseType;

            IModule IBindingBuilderComponent.Module => Module;

            BindingScope IBindingBuilderComponent.BindingScope => BindingScope;

            string IBindingBuilderComponent.Name => Name;

            protected Type BaseType { get; }

            protected IModule Module { get; }

            protected BindingScope BindingScope { get; set; }

            protected string Name { get; set; }

            internal BindingBuilderComponent(Type type, IModule module) {
                BaseType = type;
                Module = module;
            }

            public T In(BindingScope scope) {
                BindingScope = scope;
                return this as T;
            }

            public T Named(string name) {
                Name = name;
                return this as T;
            }
        }

        public class BindingBuilder : BindingBuilderComponent<BindingBuilder>, IBuilder {

            internal BindingBuilder(Type type, IModule module) : base(type, module) {
            }

            IBinding IBuilder.Build() {
                return new Binding(this);
            }
        }
    }
}