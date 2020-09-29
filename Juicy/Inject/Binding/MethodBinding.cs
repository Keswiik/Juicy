using Juicy.Constants;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Juicy.Reflection.Interfaces;
using System;

namespace Juicy.Inject.Binding {

    /// <inheritdoc cref="IMethodBinding"/>
    public sealed class MethodBinding : Binding, IMethodBinding {
        public ICachedMethod Method { get; }

        /// <summary>
        /// Consumes builder to fill out attributes.
        /// </summary>
        /// <param name="component">The component to pull method information from.</param>
        private MethodBinding(IMethodBindingComponent component) : base(component) {
            Method = component._Method;
        }

        public override string ToString() {
            return $"MethodBinding[name={Name} scope={Enum.GetName(typeof(BindingScope), Scope)} baseType={BaseType.Name} method={Method.Name}]";
        }

        // don't know much about this until runtime, can't validate.
        protected override void Validate() { }

        #region Builder

        /// <inheritdoc/>
        private interface IMethodBindingComponent : Binding.IBindingBuilderComponent {
            ICachedMethod _Method { get; }
        }

        /// <inheritdoc/>
        public abstract class MethodBindingComponent<T> : BindingComponent<T>, IMethodBindingComponent where T : MethodBindingComponent<T> {
            public ICachedMethod _Method { get; private set; }

            internal MethodBindingComponent(Type type, BindingType bindingType, IModule module) : base(type, bindingType, module) {
            }

            /// <summary>
            /// Sets the method to be bound.
            /// </summary>
            /// <param name="method">The method to bind.</param>
            /// <returns>The binding.</returns>
            public T Method(ICachedMethod method) {
                _Method = method;
                return this as T;
            }
        }

        /// <summary>
        /// Builder used to produce new method bindings.
        /// </summary>
        public sealed class MethodBindingBuilder : MethodBindingComponent<MethodBindingBuilder>, IBuilder {

            internal MethodBindingBuilder(Type type, BindingType bindingType, IModule module) : base(type, bindingType, module) {
            }

            IBinding IBuilder.Build() {
                return new MethodBinding(this);
            }
        }

        #endregion Builder
    }
}