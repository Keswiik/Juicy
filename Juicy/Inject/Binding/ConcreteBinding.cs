using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Juicy.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using Juicy.Inject.Injection.Provide;

namespace Juicy.Inject.Binding {

    ///<inheritdoc cref="IConcreteBinding"/>
    public sealed class ConcreteBinding : Binding, IConcreteBinding {

        public Type ImplementationType { get; }

        public object Instance { get; }

        public Type Provider { get; }

        /// <summary>
        /// Consumes builder to fill out attributes.
        /// </summary>
        /// <param name="component">The component to pull method information from.</param>
        private ConcreteBinding(IConcreteBindingComponent component) : base(component) {
            ImplementationType = component.ImplementationType;
            Instance = component.Instance;
            Provider = component.Provider;

            // check for untargeted bindings and update accordingly
            if (Instance == null && ImplementationType == null) {
                ImplementationType = BaseType;
            }
        }

        /// <inheritdoc/>
        private interface IConcreteBindingComponent : IBindingBuilderComponent {
            internal Type ImplementationType { get; }

            internal object Instance { get; }

            internal Type Provider { get; }
        }

        /// <inheritdoc/>
        public abstract class ConcreteBindingComponent<T> : BindingComponent<T>, IConcreteBindingComponent where T : ConcreteBindingComponent<T> {
            Type IConcreteBindingComponent.ImplementationType => ImplementationType;

            object IConcreteBindingComponent.Instance => Instance;

            Type IConcreteBindingComponent.Provider => Provider;

            private Type ImplementationType { get; set; }

            private object Instance { get; set; }

            private Type Provider { get; set; }

            internal ConcreteBindingComponent(Type type, BindingType bindingType, IModule module) : base(type, bindingType, module) { }

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
                Provider = null;
                ImplementationType = instance.GetType();
                BindingScope = BindingScope.Singleton;
                return this as T;
            }

            /// <summary>
            /// Binds <seealso cref="IBinding.BaseType"/> to a class that implements <c>IProvider</c>.
            /// </summary>
            /// <remarks>
            /// All provider bindings must use the <seealso cref="BindingType.Provider"/> binding type.
            /// </remarks>
            /// <param name="type">The type of the provider.</param>
            /// <returns>The builder.</returns>
            public T ToProvider<J>() {
                Provider = typeof(J);
                Instance = null;
                BindingType = BindingType.Provider;
                ImplementationType = BaseType;
                return this as T;
            }
        }

        /// <summary>
        /// Build used to produce new concrete bindings.
        /// </summary>
        public sealed class ConcreteBindingBuilder : ConcreteBindingComponent<ConcreteBindingBuilder>, IBuilder {
            internal ConcreteBindingBuilder(Type type, BindingType bindingType, IModule module) : base(type, bindingType, module) { }

            IBinding IBuilder.Build() {
                return new ConcreteBinding(this);
            }
        }
    }
}
