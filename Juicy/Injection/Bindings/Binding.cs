using Juicy.Injection.Interfaces;
using System;

namespace Juicy.Injection.Bindings {

    internal class Binding : IBinding {
        private Type implementationType;

        private string name;

        private object instance;

        public Binding(Type baseType) {
            BaseType = baseType;
        }

        public Type BaseType { get; }

        public Type ImplementationType {
            get => implementationType;

            set {
                if (!BaseType.IsAssignableFrom(value)) {
                    throw new ArgumentException($"{value.Name} is not a valid implementation of {BaseType.Name}");
                }

                implementationType = value;
            }
        }

        public Scope Scope { get; set; }

        public string Name {
            get => name;

            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    throw new ArgumentException("Binding name cannot be null or empty");
                }

                name = value;
            }
        }

        public object Instance {
            get => instance;

            set {
                instance = value ?? throw new ArgumentNullException("Binding instance cannot be set to null");
            }
        }
    }
}