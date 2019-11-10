using Juicy.Injection.Bindings;
using System;

namespace Juicy.Injection.Interfaces {

    public interface IBindingBuilder {

        IBindingBuilder To(Type type);

        IBindingBuilder In(Scope scope);

        IBindingBuilder Named(string name);

        IBindingBuilder ToInstance<T>(T instance);
    }
}