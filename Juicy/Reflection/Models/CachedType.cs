using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;

namespace Juicy.Reflection.Models {

    internal class CachedType : AttributeHolder, ICachedType {
        private List<ICachedMethod> constructors;

        private Dictionary<string, List<ICachedMethod>> methods;

        protected CachedType(Builder builder) : base(builder) {
            Constructors = builder._Constructors;
            Methods = builder._Methods;
            Type = builder._Type;
        }

        public List<ICachedMethod> Constructors {
            get => new List<ICachedMethod>(constructors);

            set => constructors = value;
        }

        public Dictionary<string, List<ICachedMethod>> Methods {
            get => new Dictionary<string, List<ICachedMethod>>(methods);

            set => methods = value;
        }

        public Type Type { get; private set; }

        #region Builder

        protected interface ICachedTypeComponent : IAttributeHolderComponent {
            List<ICachedMethod> _Constructors { get; }

            Dictionary<string, List<ICachedMethod>> _Methods { get; }

            Type _Type { get; }
        }

        public class CachedTypeComponent<T> : AttributeHolderComponent<T>, ICachedTypeComponent
                where T : CachedTypeComponent<T> {

            public CachedTypeComponent() {
                _Constructors = new List<ICachedMethod>();
                _Methods = new Dictionary<string, List<ICachedMethod>>();
            }

            public List<ICachedMethod> _Constructors { get; }

            public Dictionary<string, List<ICachedMethod>> _Methods { get; }

            public Type _Type { get; private set; }

            public T Constructor(ICachedMethod cachedMethod) {
                _Constructors.Add(cachedMethod);
                return this as T;
            }

            public T Constructors(params ICachedMethod[] methods) {
                _Constructors.AddRange(methods);
                return this as T;
            }

            public T Method(ICachedMethod method) {
                AddMethod(method);
                return this as T;
            }

            public T Methods(params ICachedMethod[] methods) {
                foreach (ICachedMethod method in methods) {
                    AddMethod(method);
                }

                return this as T;
            }

            public T Type(Type type)
            {
                _Type = type;

                return this as T;
            }

            private void AddMethod(ICachedMethod method) {
                if (!_Methods.ContainsKey(method.Name)) {
                    _Methods[method.Name] = new List<ICachedMethod>();
                }

                _Methods[method.Name].Add(method);
            }
        }

        new public class Builder : CachedTypeComponent<Builder>, IBuilder<CachedType> {

            public CachedType Build() {
                return new CachedType(this);
            }
        }

        #endregion Builder
    }
}