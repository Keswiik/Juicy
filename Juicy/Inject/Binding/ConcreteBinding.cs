using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Binding {
    public sealed class ConcreteBinding : Binding, IConcreteBinding {
        public Type ImplementationType { get; }

        public object Instance { get; }

        private ConcreteBinding(IConcreteBindingComponent component) : base(component) {
            ImplementationType = component.ImplementationType;
            Instance = component.Instance;
        }

        public interface IConcreteBindingComponent : Binding.IBindingBuilderComponent {
            internal Type ImplementationType { get; }

            internal object Instance { get; }
        }

        public abstract class ConcreteBindingComponent<T> : Binding.BindingBuilderComponent<T>, IConcreteBindingComponent where T : class, IConcreteBindingComponent {
            Type IConcreteBindingComponent.ImplementationType => ImplementationType;

            object IConcreteBindingComponent.Instance => Instance;

            private Type ImplementationType { get; set; }

            private object Instance { get; set; }

            internal ConcreteBindingComponent(Type type, IModule module) : base(type, module) { }

            public T To<J>() {
                ImplementationType = typeof(J);
                return this as T;
            }

            public T ToInstance(object instance) {
                Instance = instance;
                ImplementationType = instance.GetType();
                BindingScope = Constants.BindingScope.Singleton;
                return this as T;
            }
        }

        public sealed class ConcreteBindingBuilder : ConcreteBindingComponent<ConcreteBindingBuilder>, IBuilder {
            internal ConcreteBindingBuilder(Type type, IModule module) : base(type, module) { }

            IBinding IBuilder.Build() {
                return new ConcreteBinding(this);
            }
        }
    }
}
