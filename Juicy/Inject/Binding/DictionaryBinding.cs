using Juicy.Constants;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;
using static Juicy.Inject.Binding.DictionaryBinding;

namespace Juicy.Inject.Binding {
    public sealed class DictionaryBinding : Binding, IDictionaryBinding {

        public Dictionary<object, Type> ImplementationTypes { get; }

        private DictionaryBinding(IDictionaryBindingComponent component) : base(component) {
            ImplementationTypes = component.ImplementationTypes;
        }

        #region Builder

        private interface IDictionaryBindingComponent : IBindingBuilderComponent {
            internal Dictionary<object, Type> ImplementationTypes { get; }
        }

        public class DictionaryBindingComponent<T> : BindingComponent<T>, IDictionaryBindingComponent where T : DictionaryBindingComponent<T> {
            Dictionary<object, Type> IDictionaryBindingComponent.ImplementationTypes => ImplementationTypes;

            public Dictionary<object, Type> ImplementationTypes { get; }

            internal DictionaryBindingComponent(Type type, BindingType bindingType, IModule module) : base(type, bindingType, module) {
                ImplementationTypes = new Dictionary<object, Type>();
            }

            public T To(object key, Type value) {
                ImplementationTypes[key] = value;
                return this as T;
            }
        }

        public class DictionaryBindingBuilder : DictionaryBindingComponent<DictionaryBindingBuilder>, IBuilder {
            internal DictionaryBindingBuilder(Type type, BindingType bindingType, IModule module) : base(type, bindingType, module) { }

            IBinding IBuilder.Build() {
                return new DictionaryBinding(this);
            }
        }

        #endregion
    }
}
