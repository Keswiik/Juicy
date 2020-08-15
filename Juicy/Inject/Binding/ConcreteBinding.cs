using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Juicy.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Binding {

    ///<inheritdoc cref="IConcreteBinding"/>
    public sealed class ConcreteBinding : Binding, IConcreteBinding {
        public Type ImplementationType { get; }

        public object Instance { get; }

        /// <summary>
        /// Consumes builder to fill out attributes.
        /// </summary>
        /// <param name="component">The component to pull method information from.</param>
        private ConcreteBinding(IConcreteBindingComponent component) : base(component) {
            ImplementationType = component.ImplementationType;
            Instance = component.Instance;
        }

        /// <inheritdoc/>
        private interface IConcreteBindingComponent : IBindingBuilderComponent {
            internal Type ImplementationType { get; }

            internal object Instance { get; }
        }

        /// <inheritdoc/>
        public abstract class ConcreteBindingComponent<T> : BindingComponent<T>, IConcreteBindingComponent where T : ConcreteBindingComponent<T> {
            Type IConcreteBindingComponent.ImplementationType => ImplementationType;

            object IConcreteBindingComponent.Instance => Instance;

            private Type ImplementationType { get; set; }

            private object Instance { get; set; }

            internal ConcreteBindingComponent(Type type, IModule module) : base(type, module) { }

            /// <summary>
            /// Sets the implementation type for non-instance bindings.
            /// </summary>
            /// <typeparam name="J">The implementation to bind to.</typeparam>
            /// <returns>The builder.</returns>
            public T To<J>() {
                ImplementationType = typeof(J);
                return this as T;
            }

            /// <summary>
            /// Sets the instance to be explicitly bound to.
            /// </summary>
            /// <remarks>
            /// All instance bindings <b>must</b> be <see cref="BindingScope.Singleton"/> scoped.
            /// </remarks>
            /// <param name="instance">The instance to bind to.</param>
            /// <returns>The builder.</returns>
            public T ToInstance(object instance) {
                Instance = instance;
                ImplementationType = instance.GetType();
                BindingScope = BindingScope.Singleton;
                return this as T;
            }
        }

        /// <summary>
        /// Build used to produce new concrete bindings.
        /// </summary>
        public sealed class ConcreteBindingBuilder : ConcreteBindingComponent<ConcreteBindingBuilder>, IBuilder {
            internal ConcreteBindingBuilder(Type type, IModule module) : base(type, module) { }

            IBinding IBuilder.Build() {
                return new ConcreteBinding(this);
            }
        }
    }
}
