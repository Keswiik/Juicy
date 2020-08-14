using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Binding {
    public sealed class FactoryBinding : Binding, IFactoryBinding {
        public Type GenericType { get; }

        public Type ImplementationType { get; }

        private FactoryBinding(IFactoryBindingComponent component) : base(component) {
            GenericType = component.GenericType;
            ImplementationType = component.ImplementationType;
        }

        public interface IFactoryBindingComponent : Binding.IBindingBuilderComponent {
            internal Type GenericType { get; }
            internal Type ImplementationType { get; }
        }

        public abstract class FactoryBindingComponent<T> : BindingBuilderComponent<T>, IFactoryBindingComponent where T : class, IFactoryBindingComponent {
            Type IFactoryBindingComponent.GenericType => GenericType;

            Type IFactoryBindingComponent.ImplementationType => ImplementationType;

            private Type GenericType { get; set; }

            private Type ImplementationType { get; set; }

            internal FactoryBindingComponent(Type type, IModule module) : base(type, module) { }

            public T Implement<G, I>() where I : G {
                GenericType = typeof(G);
                ImplementationType = typeof(I);
                return this as T;
            }
        }

        public sealed class FactoryBindingBuilder : FactoryBindingComponent<FactoryBindingBuilder>, IBuilder {
            internal FactoryBindingBuilder(Type type, IModule module) : base(type, module) { }

            IBinding IBuilder.Build() {
                return new FactoryBinding(this);
            }
        }
    }
}
