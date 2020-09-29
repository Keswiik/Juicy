using Juicy.Constants;
using Juicy.Inject.Exceptions;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;

namespace Juicy.Inject.Binding {

    /// <inheritdoc cref="IDictionaryBinding"/>
    public sealed class DictionaryBinding : Binding, IDictionaryBinding {
        public Dictionary<object, Type> ImplementationTypes { get; }

        private DictionaryBinding(IDictionaryBindingComponent component) : base(component) {
            ImplementationTypes = component.ImplementationTypes;

            Validate();
        }

        public override string ToString() {
            return $"DictionaryBinding[name={Name} scope={Enum.GetName(typeof(BindingScope), Scope)} dictionaryType={BaseType.Name} implementationCount={ImplementationTypes.Count}]";
        }

        protected override void Validate() {
            var keyType = BaseType.GenericTypeArguments[0];
            var type = BaseType.GenericTypeArguments[1];

            foreach (var key in ImplementationTypes.Keys) {
                if (!keyType.IsAssignableFrom(key.GetType())) {
                    throw new InvalidBindingException($"Key of type {key.GetType().Name} cannot be assigned to {keyType.Name}.");
                } else if (!type.IsAssignableFrom(ImplementationTypes[key])) {
                    throw new InvalidOperationException($"Value of type {ImplementationTypes[key].Name} is not a subclass of the base type {type.Name}.");
                } else if (ImplementationTypes[key].IsInterface || ImplementationTypes[key].IsAbstract) {
                    throw new InvalidBindingException($"{type.Name} cannot be instantiated, it is either an interface or abstract class.");
                }
            }
        }

        #region Builder

        private interface IDictionaryBindingComponent : IBindingBuilderComponent {
            Dictionary<object, Type> ImplementationTypes { get; }
        }

        public class DictionaryBindingComponent<T> : BindingComponent<T>, IDictionaryBindingComponent where T : DictionaryBindingComponent<T> {
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

            internal DictionaryBindingBuilder(Type type, BindingType bindingType, IModule module) : base(type, bindingType, module) {
            }

            IBinding IBuilder.Build() {
                return new DictionaryBinding(this);
            }
        }

        #endregion Builder
    }
}