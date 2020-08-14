using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

 namespace Juicy.Reflection.Models {

    internal class CachedMethod : AttributeHolder, ICachedMethod {
        private readonly MethodBase methodBase;

        private List<ICachedParameter> parameters;

        protected CachedMethod(ICachedMethodComponent component) : base(component) {
            Name = component._Name;
            ReturnType = component._ReturnType;
            Parameters = component._Parameters;
            methodBase = component._MethodBase;
        }

        public string Name { get; }

        public Type ReturnType { get; }

        public List<ICachedParameter> Parameters {
            get => new List<ICachedParameter>(parameters);

            set => parameters = value;
        }

        public T Invoke<T>(object instance, params object[] args) {
            Type type = typeof(T);
            if (!type.IsAssignableFrom(ReturnType)) {
                throw new InvalidOperationException($"Cannot invoke method {Name}, return type of {ReturnType.Name} cannot be cast to {type.Name}");
            }

            object value = Invoke(instance, args);

            if (value == null) {
                return default;
            }

            return (T)value;
        }

        public object Invoke(object instance, params object[] args) {
            return methodBase.IsConstructor ? //
                (methodBase as ConstructorInfo).Invoke(args) : //
                methodBase.Invoke(instance, args);
        }

        #region Builder

        protected interface ICachedMethodComponent : IAttributeHolderComponent {
            string _Name { get; }

            Type _ReturnType { get; }

            List<ICachedParameter> _Parameters { get; }

            MethodBase _MethodBase { get; }
        }

        public class CachedMethodComponent<T> : AttributeHolderComponent<T>, ICachedMethodComponent
                where T : CachedMethodComponent<T> {
            public string _Name { get; private set; }

            public Type _ReturnType { get; private set; }

            public List<ICachedParameter> _Parameters { get; }

            public MethodBase _MethodBase { get; private set; }

            public CachedMethodComponent() {
                _Parameters = new List<ICachedParameter>();
            }

            public T Name(string name) {
                _Name = name;
                return this as T;
            }

            public T ReturnType(Type type) {
                _ReturnType = type;
                return this as T;
            }

            public T Parameter(ICachedParameter parameter) {
                _Parameters.Add(parameter);
                return this as T;
            }

            public T Parameters(params ICachedParameter[] parameters) {
                foreach (ICachedParameter parameter in parameters) {
                    _Parameters.Add(parameter);
                }

                return this as T;
            }

            public T MethodBase(MethodBase methodBase) {
                _MethodBase = methodBase;
                return this as T;
            }
        }

        new public class Builder : CachedMethodComponent<Builder>, IBuilder<CachedMethod> {

            public CachedMethod Build() {
                return new CachedMethod(this);
            }
        }

        #endregion Builder
    }
}