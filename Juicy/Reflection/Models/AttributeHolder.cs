using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;

 namespace Juicy.Reflection.Models {

    internal class AttributeHolder : IAttributeHolder {
        private Dictionary<Type, List<Attribute>> attributes;

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
            } else if (!type.IsAssignableFrom(selectedAttribute.GetType())) {
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
                if (!type.IsAssignableFrom(attribute.GetType())) {
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

        #region Builder

        protected interface IAttributeHolderComponent {
            Dictionary<Type, List<Attribute>> Attributes { get; }
        }

        internal class AttributeHolderComponent<T> : IAttributeHolderComponent
                where T : AttributeHolderComponent<T> {
            private readonly Dictionary<Type, List<Attribute>> attributes;

            public AttributeHolderComponent() {
                attributes = new Dictionary<Type, List<Attribute>>();
            }

            public Dictionary<Type, List<Attribute>> Attributes => attributes;

            public T AddAttribute(Attribute attribute) {
                AddOrCreateAttribute(attribute);
                return this as T;
            }

            public T AddAttributes(params Attribute[] attributes) {
                foreach (Attribute attribute in attributes) {
                    AddOrCreateAttribute(attribute);
                }

                return this as T;
            }

            private void AddOrCreateAttribute(Attribute attribute) {
                Type attributeType = attribute.GetType();
                if (!attributes.ContainsKey(attributeType)) {
                    attributes[attributeType] = new List<Attribute>();
                }

                attributes[attributeType].Add(attribute);
            }
        }

        public class Builder : AttributeHolderComponent<Builder>, IBuilder<AttributeHolder> {

            public AttributeHolder Build() {
                return new AttributeHolder(this);
            }
        }

        #endregion Builder
    }
}