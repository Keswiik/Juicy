using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;

namespace Juicy.Reflection.Models {

    /// <summary>
    /// Attribute holder that allows for extensible builders and inheritance.
    /// </summary>
    internal class AttributeHolder : IAttributeHolder {
        private readonly Dictionary<Type, List<Attribute>> attributes;

        /// <summary>
        /// Internal constructor that allows for extensible builders to be used to construct subclasses.
        /// </summary>
        /// <param name="component">The component to pull attribute information from.</param>
        protected AttributeHolder(IAttributeHolderComponent component) {
            attributes = component.Attributes ?? new Dictionary<Type, List<Attribute>>();
        }

        public bool HasAttribute(Type type) {
            return attributes.ContainsKey(type) && attributes[type].Count > 0;
        }

        public T GetAttribute<T>()
                where T : Attribute {
            Type type = typeof(T);
            Attribute selectedAttribute = GetAttribute(type);
            if (selectedAttribute == null) {
                return null;
            } else if (!type.IsAssignableFrom(selectedAttribute.GetType())) { // extra type checking to make sure we don't blow anything up.
                throw new InvalidOperationException($"Attribute of type {selectedAttribute.GetType().Name} cannot be cast to type {type.Name}");
            }

            return selectedAttribute as T;
        }

        public List<T> GetAttributes<T>()
                where T : Attribute {
            Type type = typeof(T);
            List<Attribute> knownAttributes = GetAttributes(type);
            List<T> selectedAttributes = new List<T>();

            foreach (Attribute attribute in knownAttributes) {
                if (!type.IsAssignableFrom(attribute.GetType())) { // extra type checking to make sure we don't blow anything up.
                    throw new InvalidOperationException($"Attribute of type {attribute.GetType().Name} cannot be cast to type {type.Name}");
                }

                selectedAttributes.Add(attribute as T);
            }

            return selectedAttributes;
        }

        public Attribute GetAttribute(Type type) {
            if (!HasAttribute(type)) {
                return null;
            }

            List<Attribute> knownAttributes = attributes[type];
            if (knownAttributes.Count > 1) {
                throw new InvalidOperationException($"Multiple attributes of type {type.Name} are present. Cannot safely return single value.");
            }

            return knownAttributes[0];
        }

        public List<Attribute> GetAttributes(Type type) {
            if (!HasAttribute(type)) {
                return new List<Attribute>();
            }

            return new List<Attribute>(attributes[type]);
        }

        protected List<Attribute> GetAttributeWithParent<T>() {
            List<Attribute> selectedAttributes = new List<Attribute>();
            foreach (var attributeList in attributes.Values) {
                foreach (var attribute in attributeList) {
                    if (attribute is T) {
                        selectedAttributes.Add(attribute);
                    }
                }
            }

            return selectedAttributes;
        }

        #region Builder

        /// <summary>
        /// Component to be extended in sublcasses to gain access to builder properties.
        /// </summary>
        protected interface IAttributeHolderComponent {
            Dictionary<Type, List<Attribute>> Attributes { get; }
        }

        /// <summary>
        /// Builder logic used by subclasses to avoid re-implementing builder functionality downstream.
        /// </summary>
        /// <typeparam name="T">The type of the builder inheriting the component.</typeparam>
        internal class AttributeHolderComponent<T> : IAttributeHolderComponent
                where T : AttributeHolderComponent<T> {

            // TODO: implement interface explicitly to avoid exposing these to consumers.
            public Dictionary<Type, List<Attribute>> Attributes { get; }

            protected AttributeHolderComponent() {
                Attributes = new Dictionary<Type, List<Attribute>>();
            }

            /// <summary>
            /// Adds an attribute to the container.
            /// </summary>
            /// <param name="attribute">The attribute to add.</param>
            /// <returns>The builder.</returns>
            public T AddAttribute(Attribute attribute) {
                AddOrCreateAttribute(attribute);
                return this as T;
            }

            /// <summary>
            /// Adds multiple attributes to the container.
            /// </summary>
            /// <param name="attributes">The attributes to add.</param>
            /// <returns>The builder.</returns>
            public T AddAttributes(params Attribute[] attributes) {
                foreach (Attribute attribute in attributes) {
                    AddOrCreateAttribute(attribute);
                }

                return this as T;
            }

            private void AddOrCreateAttribute(Attribute attribute) {
                Type attributeType = attribute.GetType();
                if (!Attributes.ContainsKey(attributeType)) {
                    Attributes[attributeType] = new List<Attribute>();
                }

                Attributes[attributeType].Add(attribute);
            }
        }

        /// <summary>
        /// The builder used to produce new attribute holders.
        /// </summary>
        public sealed class Builder : AttributeHolderComponent<Builder>, IBuilder<AttributeHolder> {

            public AttributeHolder Build() {
                return new AttributeHolder(this);
            }
        }

        #endregion Builder
    }
}