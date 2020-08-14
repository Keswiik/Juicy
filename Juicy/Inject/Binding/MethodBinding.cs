using Juicy.Interfaces.Binding;
using Juicy.Interfaces.Injection;
using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Binding {
    public sealed class MethodBinding : Binding, IMethodBinding {

        public ICachedMethod Method { get; }

        private MethodBinding(IMethodBindingComponent component) : base(component) {
            Method = component._Method;
        }

        public interface IMethodBindingComponent : Binding.IBindingBuilderComponent {
            internal ICachedMethod _Method { get; }
        }

        public abstract class MethodBindingComponent<T> : Binding.BindingBuilderComponent<T>, IMethodBindingComponent where T : class, IMethodBindingComponent {
            ICachedMethod IMethodBindingComponent._Method => _Method;

            private ICachedMethod _Method { get; set; }

            internal MethodBindingComponent(Type type, IModule module) : base(type, module) { }

            public T Method(ICachedMethod method) {
                _Method = method;
                return this as T;
            }
        }

        public sealed class MethodBindingBuilder : MethodBindingComponent<MethodBindingBuilder>, IBuilder {
            internal MethodBindingBuilder(Type type, IModule module) : base(type, module) { }

            IBinding IBuilder.Build() {
                return new MethodBinding(this);
            }
        }
    }
}
