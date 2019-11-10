using Juicy.Injection.Interfaces;
using System;

namespace Juicy.Injection.Bindings {

    internal class BindingBuilder : IBindingBuilder {
        private readonly IBinding binding;

        public BindingBuilder(IBinding binding) {
            this.binding = binding;
        }

        public IBindingBuilder In(Scope scope) {
            binding.Scope = scope;
            return this;
        }

        public IBindingBuilder Named(string name) {
            binding.Name = name;
            return this;
        }

        public IBindingBuilder To(Type type) {
            binding.ImplementationType = type;
            return this;
        }

        public IBindingBuilder ToInstance<T>(T instance) {
            binding.Instance = instance;
            return this;
        }
    }
}