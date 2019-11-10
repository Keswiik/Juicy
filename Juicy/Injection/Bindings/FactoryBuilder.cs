using Juicy.Injection.Interfaces;
using System;

namespace Juicy.Injection.Bindings {

    internal class FactoryBuilder : IFactoryBuilder {
        internal Type BaseType { get; private set; }

        internal Type ImplementationType { get; private set; }

        internal Type FactoryType { get; private set; }

        public IFactoryBuilder Implement(Type baseType, Type implementationType) {
            if (!baseType.IsAssignableFrom(implementationType)) {
                throw new ArgumentException($"Implementation type {implementationType.Name} cannot be cast to {baseType.Name}");
            }

            BaseType = baseType;
            ImplementationType = implementationType;

            return this;
        }

        public IFactoryBuilder Build(Type factoryType) {
            if (!factoryType.IsInterface) {
                throw new ArgumentException($"Factory type {factoryType.Name} is not an interface.");
            }

            FactoryType = factoryType;

            return this;
        }
    }
}