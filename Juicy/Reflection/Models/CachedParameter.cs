using Juicy.Inject.Binding.Attributes;
using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;

namespace Juicy.Reflection.Models {

    ///<inheritdoc cref="ICachedParameter"/>
    internal sealed class CachedParameter : AttributeHolder, ICachedParameter {

        /// <summary>
        /// Consumes builder to fill out attributes.
        /// </summary>
        /// <param name="component">The component to pull method information from.</param>
        private CachedParameter(ICachedParameterComponent component) : base(component) {
            Type = component._Type;
            Position = component._Position;

            // find attribute information
            Name = GetAttribute<NamedAttribute>()?.Name;
            if (Name == null) {
                List<Attribute> bindingAttributes = GetAttributeWithParent<BindingAttribute>();
                if (bindingAttributes.Count == 1) {
                    Name = bindingAttributes[0].GetType().FullName;
                }
            }
        }

        public Type Type { get; }

        public int Position { get; }

        public string Name { get; }

        #region Builder

        ///<inheritdoc/>
        private interface ICachedParameterComponent : IAttributeHolderComponent {
            Type _Type { get; }

            int _Position { get; }
        }

        ///<inheritdoc/>
        public class CachedParameterComponent<T> : AttributeHolderComponent<T>, ICachedParameterComponent
                where T : CachedParameterComponent<T> {
            public Type _Type { get; private set; }

            public int _Position { get; private set; }

            /// <summary>
            /// Sets the type of the parameter.
            /// </summary>
            /// <param name="type">The type of the parameter.</param>
            /// <returns>The builder.</returns>
            public T Type(Type type) {
                _Type = type;
                return this as T;
            }

            /// <summary>
            /// Sets the position of the parameter.
            /// </summary>
            /// <param name="position">The position of the parameter.</param>
            /// <returns>The builder.</returns>
            public T Position(int position) {
                _Position = position;
                return this as T;
            }
        }

        /// <summary>
        /// Builder used to produce new cached parameters.
        /// </summary>
        new public class Builder : CachedParameterComponent<Builder>, IBuilder<CachedParameter> {

            public CachedParameter Build() {
                return new CachedParameter(this);
            }
        }

        #endregion Builder
    }
}