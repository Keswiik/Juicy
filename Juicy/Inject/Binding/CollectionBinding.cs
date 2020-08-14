using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Binding {
    public sealed class CollectionBinding : Binding, ICollectionBinding {

        public List<Type> ImplementationTypes { get; }

        private CollectionBinding(ICollectionBindingComponent component) : base(component) {
            ImplementationTypes = component.ImplementationTypes;
        }

        public interface ICollectionBindingComponent : Binding.IBindingBuilderComponent {
            internal List<Type> ImplementationTypes { get; }
        }

        public class CollectionBindingComponent<T> : Binding.BindingBuilderComponent<T>, ICollectionBindingComponent where T : class, ICollectionBindingComponent {

            List<Type> ICollectionBindingComponent.ImplementationTypes => ImplementationTypes;

            public List<Type> ImplementationTypes { get; }

            internal CollectionBindingComponent(Type type, IModule module) : base(type, module) {
                ImplementationTypes = new List<Type>();
            }

            public T To<J>() {
                ImplementationTypes.Add(typeof(J));
                return this as T;
            }
        }

        public class CollectionBindingBuilder : CollectionBindingComponent<CollectionBindingBuilder>, IBuilder {
            internal CollectionBindingBuilder(Type type, IModule module) : base(type, module) { }

            IBinding IBuilder.Build() {
                return new CollectionBinding(this);
            }
        }
    }
}
