using Juicy.Constants;
using Juicy.Inject.Exceptions;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Binding {

    /// <inheritdoc cref="IFactoryBinding"/>
    public sealed class FactoryBinding : Binding, IFactoryBinding {

        public Type GenericType { get; }

        public Type ImplementationType { get; }

        /// <summary>
        /// Consumes builder to fill out attributes.
        /// </summary>
        /// <param name="component">The component to pull method information from.</param>
        private FactoryBinding(IFactoryBindingComponent component) : base(component) {
            GenericType = component.GenericType;
            ImplementationType = component.ImplementationType;

            Validate();
        }

        protected override void Validate() {
            if (!BaseType.IsInterface) {
                throw new InvalidBindingException($"Factory type {BaseType.Name} is not an interface.");
            } else if (!GenericType.IsAssignableFrom(ImplementationType)) {
                throw new InvalidBindingException($"{ImplementationType.Name} is not a subclass of the base type {GenericType.Name}.");
            } else if (ImplementationType.IsInterface || ImplementationType.IsAbstract) {
                throw new InvalidBindingException($"{ImplementationType.Name} cannot be instantiated, it is either an interface or abstract class.");
            }
        }

        #region Builder

        /// <inheritdoc/>
        private interface IFactoryBindingComponent : IBindingBuilderComponent {

            internal Type GenericType { get; }

            internal Type ImplementationType { get; }
        }

        /// <inheritdoc/>
        public abstract class FactoryBindingComponent<T> : BindingComponent<T>, IFactoryBindingComponent where T : FactoryBindingComponent<T> {
            Type IFactoryBindingComponent.GenericType => GenericType;

            Type IFactoryBindingComponent.ImplementationType => ImplementationType;

            private Type GenericType { get; set; }

            private Type ImplementationType { get; set; }

            internal FactoryBindingComponent(Type type, BindingType bindingType, IModule module) : base(type, bindingType, module) { }

            /// <summary>
            /// Specifiy the base type and implementation type of the factory.
            /// </summary>
            /// <typeparam name="G">The base type being implemented.</typeparam>
            /// <typeparam name="I">The implementation that will be produced by the factory.</typeparam>
            /// <returns>The builder.</returns>
            public T Implement<G, I>() where I : G {
                GenericType = typeof(G);
                ImplementationType = typeof(I);
                return this as T;
            }
        }

        /// <summary>
        /// Builder used to produce new factory bindings.
        /// </summary>
        public sealed class FactoryBindingBuilder : FactoryBindingComponent<FactoryBindingBuilder>, IBuilder {
            internal FactoryBindingBuilder(Type type, BindingType bindingType, IModule module) : base(type, bindingType, module) { }

            IBinding IBuilder.Build() {
                return new FactoryBinding(this);
            }
        }

        #endregion
    }
}
