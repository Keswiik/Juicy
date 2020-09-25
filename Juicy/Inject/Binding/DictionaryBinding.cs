using Juicy.Constants;
using Juicy.Inject.Exceptions;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;
using static Juicy.Inject.Binding.DictionaryBinding;

namespace Juicy.Inject.Binding {

    /// <inheritdoc cref="IDictionaryBinding"/>
    public sealed class DictionaryBinding : Binding, IDictionaryBinding {

        public Dictionary<object, Type> ImplementationTypes { get; }

        private DictionaryBinding(IDictionaryBindingComponent component) : base(component) {
            ImplementationTypes = component.ImplementationTypes;

            Validate();
        }

        protected override void Validate() {
            var keyType = BaseType.GenericTypeArguments[0];
            var type = BaseType.GenericTypeArguments[1];

            foreach (var key in ImplementationTypes.Keys) {
                if (!keyType.IsAssignableFrom(key.GetType())) {
                    throw new InvalidBindingException($"Key of type {key.GetType().Name} cannot be assigned to {keyType.Name}.");
                } else if (!type.IsAssignableFrom(ImplementationTypes[key])) {
                    throw new InvalidOperationException($"Value of type {ImplementationTypes[key].Name} is not a subclass of the base type {type.Name}.");
                }
            }
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
