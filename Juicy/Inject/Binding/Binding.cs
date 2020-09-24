using Juicy.Constants;
using Juicy.Inject.Binding.Attributes;
using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using System;

namespace Juicy.Inject.Binding {

    ///<inheritdoc cref="IBinding"/>
    public class Binding : IBinding {
        public Type BaseType { get; }

        public BindingScope Scope { get; }

        public BindingType Type { get; }

        public string Name { get; }

        public IModule Module { get; }

        /// <summary>
        /// Internal constructor that allows for extensible builders to be used to construct subclasses.
        /// </summary>
        /// <param name="component">The component to pull attribute information from.</param>
        protected Binding(IBindingBuilderComponent component) {
            BaseType = component.BaseType;
            Scope = component.BindingScope;
            Type = component.BindingType;
            Name = component.Name;
            Module = component.Module;
        }

        #region Builder

        /// <summary>
        /// Component to be extended in sublcasses to gain access to builder properties.
        /// </summary>
        public interface IBindingBuilderComponent {
            internal Type BaseType { get; }

            internal IModule Module { get; }

            internal BindingScope BindingScope { get; }

            internal BindingType BindingType { get; }

            internal string Name { get; }
        }

        /// <summary>
        /// Builder logic used by subclasses to avoid re-implementing builder functionality downstream.
        /// </summary>
        /// <typeparam name="T">The type of the builder inheriting the component.</typeparam>
        public abstract class BindingComponent<T> : IBindingBuilderComponent where T : BindingComponent<T>, IBindingBuilderComponent {
            Type IBindingBuilderComponent.BaseType => BaseType;

            IModule IBindingBuilderComponent.Module => Module;

            BindingScope IBindingBuilderComponent.BindingScope => BindingScope;

            BindingType IBindingBuilderComponent.BindingType => BindingType;

            string IBindingBuilderComponent.Name => Name;

            protected Type BaseType { get; }

            protected IModule Module { get; }

            protected BindingScope BindingScope { get; set; }

            protected BindingType BindingType { get; set; }

            protected string Name { get; set; }

            internal BindingComponent(Type type, BindingType bindingType, IModule module) {
                BaseType = type;
                BindingType = bindingType;
                Module = module;
            }

            /// <summary>
            /// Sets the scope of the binding.
            /// </summary>
            /// <param name="scope">The scope of the binding.</param>
            /// <returns>The builder.</returns>
            public T In(BindingScope scope) {
                BindingScope = scope;
                return this as T;
            }

            /// <summary>
            /// Sets the name of the binding.
            /// </summary>
            /// <param name="name">The name of the binding.</param>
            /// <returns>The builder.</returns>
            public T Named(string name) {
                Name = name;
                return this as T;
            }

            /// <summary>
            /// Sets the name of a binding by using an attribute.
            /// </summary>
            /// <remarks>
            /// Name makes use of the attribute's full name, so attributes with the same name in different namespaces will not clash.
            /// </remarks>
            /// <typeparam name="A">The type of attribute, derived from <see cref="BindingAddtirubte"/>, to use as the name.</typeparam>
            /// <returns>The builder.</returns>
            public T Attributed<A>() where A : BindingAttribute {
                Name = typeof(A).FullName;
                return this as T;
            }
        }

        /// <summary>
        /// True "implementation" of the builder that will be used externally.
        /// </summary>
        public class BindingBuilder : BindingComponent<BindingBuilder>, IBuilder {

            protected BindingBuilder(Type type, BindingType bindingType, IModule module) : base(type, bindingType, module) {
            }

            IBinding IBuilder.Build() {
                return new Binding(this);
            }
        }

        #endregion
    }
}